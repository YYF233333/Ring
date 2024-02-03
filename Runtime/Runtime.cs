using Godot;
using System;
using RingEngine.Runtime.Script;
using RingEngine.Runtime;
using MoonSharp.Interpreter;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Diagnostics;

namespace RingEngine.Runtime
{
    public partial class Runtime : Node
    {
        // 脚本内嵌代码解释器
        public MoonSharp.Interpreter.Script codeInterpreter;
        // 脚本源代码
        public RingScript script;
        public UI UI;
        public Canvas canvas;
        // 持久化数据存储（存档、全局变量）
        public DataBase db;
        // 下一条执行的代码块index
        public int PC = 0;


        public Runtime()
        {
            codeInterpreter = new MoonSharp.Interpreter.Script();
            script = new RingScript("res://main.md");
            UI = new UI();
            UI.Name = "UI";
            AddChild(UI);
            canvas = new Canvas();
            canvas.Name = "Canvas";
            AddChild(canvas);
            db = new DataBase();
        }

        /// <summary>
        /// 运行脚本至下一个中断点
        /// </summary>
        public void Step()
        {
            Trace.Assert(PC < script.segments.Count);
            script.segments[PC].Execute(this);
            PC++;
        }
    }
}

