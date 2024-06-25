namespace RingEngine.Runtime.AVGRuntime;

using System;
using Godot;
using RingEngine.Runtime.AVGRuntime.Effect;
using RingEngine.Runtime.AVGRuntime.Script;
using RingEngine.Runtime.Storage;

public partial class AVGRuntime : Node2D, ISubRuntime
{
    public string RuntimeName => "AVGRuntime";

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

    // 持久化数据存储
    public DataBase global;

    /// <summary>
    /// 下一条执行的代码块index
    /// </summary>
    public int PC
    {
        get => global.PC;
        set => global.PC = value;
    }

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
        global = new DataBase();
    }

    public ISnapshot Save()
    {
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
        global = DataBase.Deserialize(snapshotConcrete.global);
    }

    public void GetMessage(string runtimeName, object message)
    {
        GD.Print($"{runtimeName} send message {message}");
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
                LoadSnapshot(global.LoadHistory(1));
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
}
