using Godot;
using RingEngine.Runtime.Effect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// 由于不知名原因Canvas的子节点无法在Canvas外部访问
/// </summary>
public partial class Canvas : Node2D
{
    public Dictionary<string, Sprite2D> childs;

    public Sprite2D BG;

    public Canvas()
    {
        // 占位BG
        var bg = new Sprite2D();
        AddChild(bg);
        childs = [];
        BG = bg;
    }

    public Sprite2D changeBG(Texture2D texture)
    {
        var oldBG = BG;
        BG = new Sprite2D();
        BG.Texture = texture;
        BG.ZIndex = -1;
        BG.Centered = false;
        AddChild(BG);
        return oldBG;
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
    }

    public void RenameTexture(string name, string newName)
    {
        var child = childs[name];
        child.Name = newName;
        childs.Remove(name);
        childs[newName] = child;
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

    public void ApplyEffect(string name, IEffect effect)
    {
        effect.Apply(childs[name]);
    }

    public void ApplyEffect(string name, EffectFunc effect)
    {
        effect(childs[name]);
    }

    public void ApplyEffect(Node node, IEffect effect)
    {
        effect.Apply(node);
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
