using Godot;
using System;
using System.Collections.Generic;

public partial class Canvas : Node2D
{
    List<Node2D> childs;

    public Canvas()
    {
        this.childs = [];
    }

    public void AddTexture(string name, Texture2D texture, Vector2 position, int zIndex = 0, bool centered = false)
    {
        var child = new Sprite2D();
        child.Name = name;
        child.Texture = texture;
        child.ZIndex = zIndex;
        child.Centered = centered;
        child.Position = position;
        childs.Add(child);
        AddChild(child);
    }

    public void RemoveTexture(string name)
    {
        foreach (var child in childs)
        {
            if (child.Name == name)
            {
                RemoveChild(child);
                child.QueueFree();
                break;
            }
        }
    }
}
