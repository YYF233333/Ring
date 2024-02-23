using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using RingEngine.Runtime.Effect;

public partial class Canvas : Node2D
{
    Dictionary<string, Sprite2D> childs = [];
    public Sprite2D Mask;

    public Sprite2D this[string name] => childs[name];

    public Canvas()
    {
        // 占位BG
        AddTexture("BG", GD.Load<Texture2D>("res://assets/Runtime/black.png"), Placement.BG, -1);
        Mask = new Sprite2D
        {
            Name = "Mask",
            Texture = null,
            ZIndex = 1,
            Centered = false,
        };
        AddChild(Mask);
        Mask.Owner = this;
    }

    public Texture2D Stretch(Texture2D texture)
    {
        var windowSize = GetTree().Root.Size;
        var image = texture.GetImage();
        var imageSize = image.GetSize();
        var scale = Math.Max(windowSize.X / (float)imageSize.X, windowSize.Y / (float)imageSize.Y);
        scale = Math.Max(scale, 1);
        image.Resize((int)(imageSize.X * scale), (int)(imageSize.Y * scale));
        return ImageTexture.CreateFromImage(image);
    }

    public void AddMask(Texture2D texture)
    {
        Mask.Texture = texture;
    }

    public void RemoveMask()
    {
        Mask.Texture = null;
    }

    public void AddTexture(string name, Texture2D texture, Placement placement, int zIndex = 0, bool centered = false)
    {
        var child = new Sprite2D
        {
            Name = name,
            Texture = texture,
            ZIndex = zIndex,
            Centered = centered,
            Position = new Vector2(placement.x, 200.0f),
            Scale = new Vector2(placement.scale, placement.scale)
        };
        childs[name] = child;
        AddChild(child);
        child.Owner = this;
        Trace.Assert(child.Name == name);
    }

    public Sprite2D ReplaceBG(Texture2D texture)
    {
        var child = childs["BG"];
        child.Name = "zombieBG";
        var newChild = new Sprite2D
        {
            Name = "BG",
            Texture = texture,
            ZIndex = child.ZIndex,
            Centered = child.Centered,
            Position = child.Position,
            Scale = child.Scale
        };
        AddChild(newChild);
        newChild.Owner = this;
        childs["BG"] = newChild;
        return child;
    }

    public void RemoveTexture(string name)
    {
        if (childs.TryGetValue(name, out var value))
        {
            var child = value;
            RemoveChild(child);
            child.QueueFree();
            childs.Remove(name);
        }
    }

    public void RemoveAll(bool includeBG = true)
    {
        foreach (var child in childs.Values)
        {
            RemoveChild(child);
            child.QueueFree();
        }
        childs = includeBG ? [] : new() { { "BG", childs["BG"] } };
    }

    public PackedScene Serialize()
    {
        var scene = new PackedScene();
        scene.Pack(this);
        return scene;
    }
}
