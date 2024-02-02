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
        // �ű���Ƕ���������
        public MoonSharp.Interpreter.Script codeInterpreter;
        // �ű�Դ����
        public RingScript script;
        public UI UI;
        public Canvas canvas;
        // �־û����ݴ洢���浵��ȫ�ֱ�����
        public DataBase db;
        // �زĹ���
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
        /// ���нű�����һ���жϵ�
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

