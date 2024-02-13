using Godot;

public partial class main : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //GetNode<Runtime>("/root/Runtime").Step();
        //GetNode<Runtime>("/root/Runtime").Step();
        //var canvas = new Canvas();
        //GetNode<Runtime>("/root/Runtime").canvas.AddTexture("红叶", GD.Load<Texture2D>("res://assets/chara.png"), Placements.Get("farleft"));
        //AddChild(canvas);
        //canvas.ApplyEffect("红叶", new Dissolve(2.0));

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
