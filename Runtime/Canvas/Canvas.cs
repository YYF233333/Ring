using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Godot;
using RingEngine.Runtime.Effect;
using RingEngine.Runtime.Storage;

public partial class Canvas : Node2D
{
    public GlobalConfig config;
    Dictionary<string, Sprite2D> childs = [];
    public Sprite2D Mask;
    public Sprite2D BG;
    public Sprite2D ZombieBG;

    public Sprite2D this[string name] => childs[name];

    public bool HasChild(string name) => childs.ContainsKey(name);

    public Canvas() { }

    public override void _Ready()
    {
        if (GetChildCount() > 0)
        {
            // 存档恢复
            foreach (var child in GetChildren())
            {
                switch (child.Name)
                {
                    case "BG":
                        BG = child as Sprite2D;
                        break;
                    case "Mask":
                        Mask = child as Sprite2D;
                        break;
                    default:
                        childs[child.Name] = child as Sprite2D;
                        break;
                }
            }
            return;
        }
        // 占位BG
        AddTexture("BG", GD.Load<Texture2D>("res://assets/Runtime/black.png"), Placement.BG, -1);
        BG = new Sprite2D
        {
            Name = "BG",
            Texture = GD.Load<Texture2D>("res://assets/Runtime/black.png"),
            ZIndex = -1,
            Centered = false,
            Position = Placement.BG.Position,
            Scale = new Vector2(Placement.BG.scale, Placement.BG.scale)
        };
        AddChild(BG);
        BG.Owner = this;
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

    public static void OverwriteTexture(string path, Image image)
    {
        using var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
        switch (Path.GetExtension(path))
        {
            case ".png":
                image.SavePng(path);
                break;
            case ".webp":
                image.SaveWebp(path);
                break;
            case ".jpg":
                image.SaveJpg(path);
                break;
            default:
                GD.Print($"Unknown image format {Path.GetExtension(path)}, skip.");
                break;
        }
    }

    public Texture2D Stretch(Texture2D texture, Vector2I? windowSize = null)
    {
        var _windowSize = windowSize ?? GetTree().Root.Size;
        var image = texture.GetImage();
        var imageSize = image.GetSize();
        var scale = Math.Max(
            _windowSize.X / (float)imageSize.X,
            _windowSize.Y / (float)imageSize.Y
        );
        if (scale != 1)
        {
            GD.Print($"stretch {texture.ResourcePath}, scale {scale}");
            image.Resize((int)(imageSize.X * scale), (int)(imageSize.Y * scale));
            OverwriteTexture(texture.ResourcePath, image);
            return ImageTexture.CreateFromImage(image);
        }
        else
        {
            return texture;
        }
    }

    public void AddMask(Texture2D texture) => Mask.Texture = texture;

    public void RemoveMask() => Mask.Texture = null;

    public void AddTexture(
        string name,
        Texture2D texture,
        Placement placement,
        int zIndex = 0,
        bool centered = false
    )
    {
        //Trace.Assert(texture.GetSize() == config.CharacterSize);
        var child = new Sprite2D
        {
            Name = name,
            Texture = texture,
            ZIndex = zIndex,
            Centered = centered,
            Position = new Vector2(placement.x, placement.y),
            Scale = new Vector2(placement.scale, placement.scale)
        };
        childs[name] = child;
        AddChild(child);
        child.Owner = this;
        Trace.Assert(child.Name == name);
    }

    public void RenameTexture(string name, string newName)
    {
        Trace.Assert(childs.ContainsKey(name) && !childs.ContainsKey(newName));
        var child = childs[name];
        child.Name = newName;
        childs.Remove(name);
        childs[newName] = child;
    }

    public Sprite2D ReplaceBG(Texture2D texture)
    {
        BG.Name = "zombieBG";
        ZombieBG = BG;
        BG = new Sprite2D
        {
            Name = "BG",
            Texture = texture,
            ZIndex = ZombieBG.ZIndex,
            Centered = ZombieBG.Centered,
            Position = ZombieBG.Position,
            Scale = ZombieBG.Scale
        };
        AddChild(BG);
        BG.Owner = this;
        return ZombieBG;
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
