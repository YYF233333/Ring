using Godot;
using System;
using System.IO;

public partial class main : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var canvas = new Canvas();
        canvas.AddTexture("��Ҷ", GD.Load<Texture2D>("res://assets/chara.png"), Vector2.Zero);
        //GD.Print(canvas.Serialize());
        //File.AppendAllText("C:\\code\\Ring\\log", canvas.Serialize());
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
