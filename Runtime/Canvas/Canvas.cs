using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using Godot;
using RingEngine.Runtime.Effect;

public partial class Canvas : Node2D
{
    Dictionary<string, Sprite2D> childs = [];

    public Sprite2D this[string name] => childs[name];

    public Canvas()
    {
        // 占位BG
        AddTexture("BG", GD.Load<Texture2D>("res://assets/Runtime/black.png"), Placements.BG, -1);
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

    public void AddTexture(string name, Texture2D texture, Placement placement, int zIndex = 0, bool centered = false)
    {
        var child = new Sprite2D
        {
            Name = name,
            Texture = texture,
            ZIndex = zIndex,
            Centered = centered,
            Position = placement.position,
            Scale = new Vector2(placement.scale, placement.scale)
        };
        childs[name] = child;
        AddChild(child);
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

    public string Serialize()
    {
        Dictionary<string, byte[]> childs_bin = [];
        foreach (var pair in childs)
        {
            childs_bin[pair.Key] = GD.VarToBytesWithObjects(pair.Value);
        }
        return JsonSerializer.Serialize(childs_bin);
    }

    public void Deserialize(string serializedData)
    {
        var childs_bin = (Dictionary<string, byte[]>)JsonSerializer.Deserialize(serializedData, typeof(Dictionary<string, byte[]>));
        foreach (var pair in childs_bin)
        {
            var child = (Sprite2D)GD.BytesToVarWithObjects(pair.Value);
            childs[pair.Key] = child;
            child.Name = pair.Key;
            AddChild(child);
        }
    }
}
