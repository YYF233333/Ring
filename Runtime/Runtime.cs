using Godot;
using System;
using RingEngine.Runtime.Script;
using RingEngine.Runtime;
using MoonSharp.Interpreter;
using System.Text.Json.Serialization;
using System.Text.Json;

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
        // 素材管理
        public Assets assets;


        public Runtime()
        {
            codeInterpreter = new MoonSharp.Interpreter.Script();
            script = new RingScript("");
            UI = new UI();
            canvas = new Canvas();
            db = new DataBase();
            assets = new Assets();
        }

        /// <summary>
        /// 运行脚本至下一个中断点
        /// </summary>
        public void Step()
        {

        }
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }
    }
}

