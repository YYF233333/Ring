namespace RingEngine.Runtime;
using System;
using Godot;
using RingEngine.Runtime.Effect;
using RingEngine.Runtime.Script;
using RingEngine.Runtime.Storage;

public partial class Runtime : Node2D
{
    // 脚本内嵌代码解释器
    public LuaInterpreter interpreter;
    // 脚本源代码
    public RingScript script;
    public UI UI;
    public Canvas canvas;
    // 动画效果缓冲区
    public EffectBuffer mainBuffer;
    public EffectBuffer nonBlockingBuffer;
    // 持久化数据存储（存档、全局变量）
    public DataBase db;
    // 下一条执行的代码块index
    public int PC = 0;


    public Runtime()
    {
        interpreter = new LuaInterpreter(FileAccess.GetFileAsString("res://init.lua"));
        script = new RingScript("res://main.md");
        UI = GD.Load<PackedScene>("res://Runtime/UI/UI.tscn").Instantiate<UI>();
        UI.Name = "UI";
        // 强制显示在canvas之上
        UI.ZIndex = 1;
        AddChild(UI);
        canvas = new Canvas
        {
            Name = "Canvas"
        };
        AddChild(canvas);
        mainBuffer = new EffectBuffer();
        nonBlockingBuffer = new EffectBuffer();
        db = new DataBase();
    }



    /// <summary>
    /// 运行脚本至下一个中断点
    /// </summary>
    public void Step()
    {
        if (nonBlockingBuffer.IsRunning)
        {
            nonBlockingBuffer.Interrupt();
        }
        if (mainBuffer.IsRunning)
        {
            mainBuffer.Interrupt();
            return;
        }
        if (PC < script.segments.Count)
        {
            var @continue = false;
            do
            {
                @continue = script.segments[PC].@continue;
                script.segments[PC].Execute(this);
                PC++;
            } while (@continue && PC < script.segments.Count);

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

