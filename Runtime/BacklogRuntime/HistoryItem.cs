using Godot;

public partial class HistoryItem : HBoxContainer
{
    public Backlog Root;
    public string CharacterName
    {
        get => GetNode<Label>("Name").Text.Trim(['【', '】']);
        set => GetNode<Label>("Name").Text = "【" + value + "】";
    }

    public string Content
    {
        get => GetNode<Label>("Content").Text;
        set => GetNode<Label>("Content").Text = value;
    }

    public override void _Ready()
    {
        GuiInput += this.HistoryItem_GuiInput;
    }

    private void HistoryItem_GuiInput(InputEvent @event)
    {
        if (
            @event is InputEventMouseButton mouseClick
            && mouseClick.Pressed
            && mouseClick.ButtonIndex == MouseButton.Left
        )
        {
            Root.End(GetParent().GetChildCount() - GetIndex());
        }
    }
}
