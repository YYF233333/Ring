namespace RingEngine.Runtime.Storage;

using System;
using Godot;
using RingEngine.Runtime.AVGRuntime;

public class Snapshot : ISnapshot
{
    public PackedScene UI;
    public PackedScene Canvas;
    public string Global;

    // 历史记录可以在内存中暂存，但是不能保存到磁盘
    public Snapshot[] History = [];

    Snapshot() { }

    public Snapshot(AVGRuntime runtime)
    {
        UI = runtime.UI.Serialize();
        Canvas = runtime.canvas.Serialize();
        Global = runtime.Global.Serialize();
        History = runtime.Global.History.ToArray();
    }

    public Snapshot(string folder)
    {
        Load(folder);
    }

    public void Load(string folder)
    {
        folder = folder.TrimSuffix("/");
        var UI = ResourceLoader.Load<PackedScene>($"{folder}/UI.tscn");
        var Canvas = ResourceLoader.Load<PackedScene>($"{folder}/Canvas.tscn");
        string global;
        using (
            var file =
                FileAccess.Open($"{folder}/global.json", FileAccess.ModeFlags.Read)
                ?? throw new Exception($"Failed to open file {folder}/global.json")
        )
        {
            global = file.GetAsText();
        }

        this.UI = UI;
        this.Canvas = Canvas;
        this.Global = global;
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

        using (
            var file =
                FileAccess.Open($"{folder}/global.json", FileAccess.ModeFlags.Write)
                ?? throw new Exception($"Failed to create file {folder}/global.json")
        )
        {
            file.StoreString(Global);
        }
    }
}
