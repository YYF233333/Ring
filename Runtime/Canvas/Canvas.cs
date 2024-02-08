using Godot;
using RingEngine.Runtime.Effect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// 由于不知名原因Canvas的子节点无法在Canvas外部访问
/// </summary>
public partial class Canvas : Node2D
{
    public Dictionary<string, Sprite2D> childs = [];
    public Dictionary<Node, Tween> tweens = [];

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
        var child = new Sprite2D();
        child.Name = name;
        child.Texture = texture;
        child.ZIndex = zIndex;
        child.Centered = centered;
        child.Position = placement.position;
        child.Scale = new Vector2(placement.scale, placement.scale);
        childs[name] = child;
        AddChild(child);
        Trace.Assert(child.Name == name);
    }

    public Sprite2D ReplaceBG(Texture2D texture)
    {
        var child = childs["BG"];
        child.Name = "zombieBG";
        var newChild = new Sprite2D();
        newChild.Name = "BG";
        newChild.Texture = texture;
        newChild.ZIndex = child.ZIndex;
        newChild.Centered = child.Centered;
        newChild.Position = child.Position;
        newChild.Scale = child.Scale;
        AddChild(newChild);
        childs["BG"] = newChild;
        return child;
    }

    public void RemoveTexture(string name)
    {
        if (childs.ContainsKey(name))
        {
            var child = childs[name];
            RemoveChild(child);
            child.QueueFree();
            childs.Remove(name);
        }
    }

    public void ApplyEffect(Node node, EffectFunc effect)
    {
        if (tweens.ContainsKey(node))
        {
            var tween = tweens[node];
            if (tween.IsRunning())
            {
                tweens[node].Pause();
                tweens[node].CustomStep(114);
                tweens[node].Kill();
            }
            tweens.Remove(node);
        }
        tweens[node] = effect(node);
    }

    public void ApplyEffect(string name, IEffect effect)
    {
        ApplyEffect(childs[name], effect.Apply);
    }

    public void ApplyEffect(string name, EffectFunc effect)
    {
        ApplyEffect(childs[name], effect);
    }

    public void ApplyEffect(Node node, IEffect effect)
    {
        ApplyEffect(node, effect.Apply);
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

    public void Deserialize(string serialized_data)
    {
        var childs_bin = (Dictionary<string, byte[]>)JsonSerializer.Deserialize(serialized_data, typeof(Dictionary<string, byte[]>));
        foreach (var pair in childs_bin)
        {
            var child = (Sprite2D)GD.BytesToVarWithObjects(pair.Value);
            childs[pair.Key] = child;
            child.Name = pair.Key;
            AddChild(child);
        }
    }
}
