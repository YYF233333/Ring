using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

public partial class Canvas : Node2D
{
    Dictionary<string, Node2D> childs;

    public Canvas() { this.childs = []; }

    public void Deserialize(string serialized_data)
    {
        var childs_bin = (Dictionary<string, byte[]>)JsonSerializer.Deserialize(serialized_data, typeof(Dictionary<string, byte[]>));
        foreach (var pair in childs_bin)
        {
            var child = (Node2D)GD.BytesToVarWithObjects(pair.Value);
            childs[pair.Key] = child;
            child.Name = pair.Key;
            AddChild(child);
        }
    }

    public void AddTexture(string name, Texture2D texture, Vector2 position, int zIndex = 0, bool centered = false)
    {
        var child = new Sprite2D();
        child.Name = name;
        child.Texture = texture;
        child.ZIndex = zIndex;
        child.Centered = centered;
        child.Position = position;
        childs[name] = child;
        AddChild(child);
    }

    public void RemoveTexture(string name)
    {
        var child = childs[name];
        RemoveChild(child);
        child.QueueFree();
        childs.Remove(name);
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
}
