using System;
using Godot;
using RingEngine.Runtime;

public partial class Setting : Control, ISubRuntime
{
    public string RuntimeName => "Setting";

    public string ReturnToRuntime;

    public void GetMessage(string runtimeName, object message)
    {
        ReturnToRuntime = message as string;
    }

    public void Return()
    {
        GetParent<Runtime>().SwitchRuntime(this, ReturnToRuntime, 0);
        QueueFree();
    }
}
