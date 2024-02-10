using Godot;
using System;
using RingEngine.Runtime.Script;
using RingEngine.Runtime;
using MoonSharp.Interpreter;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Diagnostics;
using RingEngine.Runtime.Storage;
using System.Collections.Generic;

namespace RingEngine.Runtime
{
    public partial class Runtime : Node2D
    {
        // 脚本内嵌代码解释器
        public MoonSharp.Interpreter.Script codeInterpreter;
        // 脚本源代码
        public RingScript script;
        public UI UI;
        public Canvas canvas;
        public Dictionary<Node, Tween> tweens = [];
        // 持久化数据存储（存档、全局变量）
        public DataBase db;
        // 下一条执行的代码块index
        public int PC = 0;


        public Runtime()
        {
            codeInterpreter = new MoonSharp.Interpreter.Script();
            script = new RingScript("res://main.md");
            UI = GD.Load<PackedScene>("res://Runtime/UI/UI.tscn").Instantiate<UI>();
            UI.Name = "UI";
            // 强制显示在canvas之上
            UI.ZIndex = 1;
            AddChild(UI);
            canvas = new Canvas(tweens);
            canvas.Name = "Canvas";
            AddChild(canvas);
            db = new DataBase();
        }



        /// <summary>
        /// 运行脚本至下一个中断点
        /// </summary>
        public void Step()
        {
            var flag = false;
            foreach (var (node, tween) in tweens)
            {
                if (tween.IsRunning())
                {
                    tween.Pause();
                    tween.CustomStep(114);
                    tween.Kill();
                    flag = true;
                }
                tweens.Remove(node);
            }
            if (flag) { return; }
            if (PC < script.segments.Count)
            {
                script.segments[PC].Execute(this);
                PC++;
            }
            else
            {
                throw new Exception("Script Out of Bound!");
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventKey)
            {
                if (Input.IsActionPressed("ui_accept"))
                {
                    Step();
                }
            }
        }
    }
}

