using Godot;
using RingEngine.Runtime;
using RingEngine.Runtime.Storage;
using System.Collections.Generic;

public partial class Backlog : Control, ISubRuntime
{
    public string RuntimeName => "Backlog";

    public HistoryList History;

    public override void _Ready()
    {
        History = new HistoryList(GetNode<Node>("MarginContainer/ScrollContainer/VBoxContainer"));
        GuiInput += Backlog_GuiInput;
    }

    public void GetMessage(string runtimeName, object message)
    {
        var snapshots = (List<Snapshot>)message;
        foreach (var snapshot in snapshots)
        {
            var UI = snapshot.UI.Instantiate<UI>();
            var item = GD.Load<PackedScene>("res://Runtime/BacklogRuntime/HistoryItem.tscn").Instantiate<HistoryItem>();
            item.Root = this;
            item.CharacterName = UI.CharacterName;
            item.Content = UI.TextBox.Text;
            History.Add(item);
        }
        // 把滚动条拖到最下面
        var scroll = GetNode<ScrollContainer>("MarginContainer/ScrollContainer");
        GetTree().Connect("process_frame", Callable.From(() => scroll.EnsureControlVisible(History[-1])), (uint)ConnectFlags.OneShot);
    }

    /// <param name="step">要回退的步数，0表示不需要回退</param>
    public void End(int step)
    {
        GetParent<Runtime>().SwitchRuntime(this, "AVG", step);
        QueueFree();
    }

    private void Backlog_GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseClick && mouseClick.Pressed && mouseClick.ButtonIndex == MouseButton.Right)
        {
            End(0);
        }
        else if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Escape)
        {
            End(0);
        }
    }
}

public class HistoryList
{
    Node _container;

    public HistoryList(Node container) => this._container = container;

    public HistoryItem this[int index]
    {
        get => _container.GetChild(index) as HistoryItem;
        set
        {
            if (_container.GetChild(index) is HistoryItem item)
            {
                item.CharacterName = value.CharacterName;
                item.Content = value.Content;
            }
        }
    }

    public void Add(HistoryItem item) => _container.AddChild(item);
}
