using Godot;
using RingEngine.Runtime.Effect;
using RingEngine.Runtime.Script;
using System;
using System.IO;

public partial class main : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //var canvas = new Canvas();
        //canvas.AddTexture("红叶", GD.Load<Texture2D>("res://assets/chara.png"), Vector2.Zero);
        //AddChild(canvas);
        //canvas.ApplyEffect("红叶", new Dissolve(2.0));
        var script = new RingScript("./scriptdemo.md");
        GD.Print(script.folderPath);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
