using System.Linq;

namespace RingEngine.Runtime.AVGRuntime;

using System;
using System.Collections.Generic;
using Godot;
using RingEngine.Runtime.AVGRuntime.Effect;
using RingEngine.Runtime.AVGRuntime.Script;
using RingEngine.Runtime.Storage;

public partial class AVGRuntime : Node2D, ISubRuntime
{
    public string RuntimeName => "AVG";

    // 脚本内嵌代码解释器
    public PythonInterpreter interpreter;

    // 脚本源代码
    public RingScript script;
    public UI UI;
    public Canvas canvas;

    // 音乐音效
    public Audio audio;

    // 动画效果缓冲区
    public EffectBuffer mainBuffer;
    public EffectBuffer nonBlockingBuffer;

    // 全局变量
    public DataBase Global { get; private set; }

    public AVGRuntime()
    {
        script = new RingScript("res://main.md");
        UI = GD.Load<PackedScene>("res://Runtime/AVGRuntime/UI/UI.tscn").Instantiate<UI>();
        UI.Name = "UI";
        // 强制显示在canvas之上
        UI.ZIndex = 1;
        AddChild(UI);
        canvas = new Canvas { Name = "Canvas", };
        AddChild(canvas);
        audio = new Audio() { Name = "Audio" };
        AddChild(audio);
        mainBuffer = new EffectBuffer();
        nonBlockingBuffer = new EffectBuffer();
        interpreter = new PythonInterpreter(
            this,
            GlobalConfig.ProjectRoot,
            FileAccess.GetFileAsString("res://init.py")
        );
        Global = new DataBase();
    }

    public ISnapshot Save()
    {
        if (Global.IsExecuting)
        {
            Global.PC++;
            Global.IsExecuting = false;
            var ret = new Snapshot(this);
            Global.PC--;
            Global.IsExecuting = true;
            return ret;
        }
        return new Snapshot(this);
    }

    public void LoadSnapshot(ISnapshot snapshot)
    {
        var snapshotConcrete = snapshot as Snapshot;
        RemoveChild(UI);
        UI.QueueFree();
        RemoveChild(canvas);
        canvas.QueueFree();
        UI = snapshotConcrete.UI.Instantiate<UI>();
        AddChild(UI);
        canvas = snapshotConcrete.Canvas.Instantiate<Canvas>();
        AddChild(canvas);
        Global = DataBase.Deserialize(snapshotConcrete.Global);
    }

    public void GetMessage(string runtimeName, object message)
    {
        GD.Print($"{runtimeName} send message {message}");
        if (runtimeName == "VerticalBranch")
        {
            var id = (int)message;
            Global.LastChosenOptionId = id;
            Step();
        }
    }

    public void LoadSnapshot(string snapshotPath)
    {
        var snap = new Snapshot(snapshotPath);
        LoadSnapshot(snap);
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
        if (Global.PC < script.segments.Count)
        {
            var @continue = false;
            do
            {
                @continue = script.segments[Global.PC].@continue;
                // 设置flag，防止执行中调用Save
                Global.IsExecuting = true;
                script.segments[Global.PC].Execute(this);
                Global.IsExecuting = false;
                Global.PC++;
            } while (@continue && Global.PC < script.segments.Count);
            Global.History.Add(new Snapshot(this));
        }
        else
        {
            throw new Exception("Script Out of Bound!");
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey)
        {
            if (Input.IsActionPressed("ui_accept"))
            {
                Step();
            }
            if (Input.IsActionPressed("Save"))
            {
                if (nonBlockingBuffer.IsRunning)
                {
                    nonBlockingBuffer.Interrupt();
                }
                if (mainBuffer.IsRunning)
                {
                    mainBuffer.Interrupt();
                }
                DebugSnapshot();
            }
            if (Input.IsActionPressed("ui_text_backspace"))
            {
                if (nonBlockingBuffer.IsRunning)
                {
                    nonBlockingBuffer.Interrupt();
                }
                if (mainBuffer.IsRunning)
                {
                    mainBuffer.Interrupt();
                }
                LoadSnapshot(Global.LoadHistory(1));
            }
            if (Input.IsActionPressed("Load"))
            {
                var snap = new Snapshot("res://snapshot");
                LoadSnapshot(snap);
            }
        }
    }

    public void InitMiniGame(string name)
    {
        GetParent<Runtime>().SwitchRuntime(this, name, new BreakoutMessage());
    }

    public void VerticalBranch(IEnumerable<string> options) => VerticalBranch(options.ToArray());

    public void VerticalBranch(params string[] options)
    {
        GetParent<Runtime>()
            .SwitchRuntime(
                this,
                "VerticalBranch",
                options.Select((option, index) => (index, option)).ToArray(),
                SwitchMode.Pause
            );
    }

    public void HorizontalBranch(IEnumerable<string> options) =>
        HorizontalBranch(options.ToArray());

    public void HorizontalBranch(params string[] options)
    {
        throw new NotImplementedException();
    }

    public void Goto(string label)
    {
        // Execute结束后会PC++，所以这里要减1
        Global.PC = script.labels[label] - 1;
    }
}
