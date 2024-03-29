namespace RingEngine.Runtime.Storage;
using System;
using Godot;

public class Snapshot
{
    public PackedScene UI;
    public PackedScene Canvas;
    public string global;

    Snapshot() { }

    public Snapshot(Runtime runtime)
    {
        UI = runtime.UI.Serialize();
        Canvas = runtime.canvas.Serialize();
        global = runtime.global.Serialize();
    }

    public static Snapshot Load(string folder)
    {
        folder = folder.TrimSuffix("/");
        var UI = ResourceLoader.Load<PackedScene>($"{folder}/UI.tscn");
        var Canvas = ResourceLoader.Load<PackedScene>($"{folder}/Canvas.tscn");
        string global;
        using (var file = FileAccess.Open($"{folder}/global.json", FileAccess.ModeFlags.Read)
            ?? throw new Exception($"Failed to open file {folder}/global.json"))
        {
            global = file.GetAsText();
        }
        return new Snapshot
        {
            UI = UI,
            Canvas = Canvas,
            global = global
        };
    }

    public void Save(string folder)
    {
        folder = folder.TrimSuffix("/");
        var ret = DirAccess.MakeDirRecursiveAbsolute(folder);
        if (ret != Error.Ok)
        {
            throw new Exception($"Failed to create directory {folder}");
        }
        ret = ResourceSaver.Save(UI, $"{folder}/UI.tscn");
        if (ret != Error.Ok)
        {
            throw new Exception($"Failed to save UI");
        }
        ret = ResourceSaver.Save(Canvas, $"{folder}/Canvas.tscn");
        if (ret != Error.Ok)
        {
            throw new Exception($"Failed to save Canvas");
        }

        using (var file = FileAccess.Open($"{folder}/global.json", FileAccess.ModeFlags.Write)
            ?? throw new Exception($"Failed to create file {folder}/global.json"))
        {
            file.StoreString(global);
        }
    }

    public Runtime Instantiate()
    {
        throw new NotImplementedException();
    }
}
