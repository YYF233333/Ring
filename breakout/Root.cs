using System;
using System.Collections.Generic;
using Godot;
using RingEngine.Runtime;
using RingEngine.Runtime.Storage;

class BreakoutMessage
{
    public Dictionary<string, object> data;
}

public partial class Root : Node, ISubRuntime
{
    public string RuntimeName => "Breakout";

    public void GetMessage(string runtimeName, object message)
    {
        var data = ((BreakoutMessage)message).data;
        var game = GD.Load<PackedScene>("res://breakout/scenes/breakout/breakout.tscn")
            .Instantiate();
        AddChild(game);
    }

    public void EndGame()
    {
        var message = new BreakoutMessage();
        GetParent<Runtime>().SwitchRuntime(this, "AVGRuntime", message);
    }
}
