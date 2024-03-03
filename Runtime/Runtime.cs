namespace RingEngine.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    // 持久化数据存储
    public DataBase global;
    // 进度无关全局设置
    public GlobalConfig config;
    /// <summary>
    /// 下一条执行的代码块index
    /// </summary>
    public int PC { get => global.PC; set => global.PC = value; }


    public Runtime()
    {
        global = new DataBase();
        config = new GlobalConfig()
        {
            YBaseTable = new Dictionary<string, double> { { "红叶", 600 } }
        };
        script = new RingScript("res://main.md");
        UI = GD.Load<PackedScene>("res://Runtime/UI/UI.tscn").Instantiate<UI>();
        UI.Name = "UI";
        // 强制显示在canvas之上
        UI.ZIndex = 1;
        AddChild(UI);
        canvas = new Canvas
        {
            Name = "Canvas",
            // 理论上class变量传引用，修改Runtime.config会同步更新Canvas.conifg
            config = config
        };
        AddChild(canvas);
        mainBuffer = new EffectBuffer();
        nonBlockingBuffer = new EffectBuffer();
        interpreter = new LuaInterpreter(this, FileAccess.GetFileAsString("res://init.lua"));
    }

    public void DebugSnapshot()
    {
        var snap = new Snapshot(this);
        snap.Save("res://snapshot");
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
            global.history.Add(new Snapshot(this));
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
            if (Input.IsActionPressed("ui_cancel"))
            {
                DebugSnapshot();
            }
        }
    }
}

