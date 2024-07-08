using System.Diagnostics;
using Godot;
using RingEngine.Runtime;

public partial class VerticalBranch : Control, ISubRuntime
{
    public string RuntimeName => "VerticalBranch";

    static string ContainerPath => "MarginContainer/CenterContainer/VBoxContainer";

    static GDScript BranchOption =>
        GD.Load<GDScript>("res://Runtime/BranchRuntime/BranchOption.gd");

    public void GetMessage(string runtimeName, object message)
    {
        Trace.Assert(runtimeName == "AVG");
        var options = ((int, string)[])message;
        var container = GetNode<Control>(ContainerPath);

        // 清空所有旧选项（主要是场景中自带的）
        foreach (var child in container.GetChildren())
        {
            container.RemoveChild(child);
            child.QueueFree();
        }

        foreach (var (id, text) in options)
        {
            var option = (Button)BranchOption.New(id, text);
            option.Connect("option_choosed", Callable.From<int>(End));
            GetNode<Control>(ContainerPath).AddChild(option);
        }
    }

    public void End(int id)
    {
        GetParent<Runtime>().SwitchRuntime(this, "AVG", id);
        QueueFree();
    }
}
