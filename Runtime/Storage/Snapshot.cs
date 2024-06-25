namespace RingEngine.Runtime.Storage;

using System;
using Godot;
using RingEngine.Runtime.AVGRuntime;

public class Snapshot : ISnapshot
{
    public PackedScene UI;
    public PackedScene Canvas;
    public string global;

    Snapshot() { }

    public Snapshot(AVGRuntime runtime)
    {
        UI = runtime.UI.Serialize();
        Canvas = runtime.canvas.Serialize();
        global = runtime.global.Serialize();
    }

    public Snapshot(string Folder)
    {
        Load(Folder);
    }

    public void Load(string Folder)
    {
        Folder = Folder.TrimSuffix("/");
        var UI = ResourceLoader.Load<PackedScene>($"{Folder}/UI.tscn");
        var Canvas = ResourceLoader.Load<PackedScene>($"{Folder}/Canvas.tscn");
        string global;
        using (
            var file =
                FileAccess.Open($"{Folder}/global.json", FileAccess.ModeFlags.Read)
                ?? throw new Exception($"Failed to open file {Folder}/global.json")
        )
        {
            global = file.GetAsText();
        }

        this.UI = UI;
        this.Canvas = Canvas;
        this.global = global;
    }

    public void Save(string Folder)
    {
        Folder = Folder.TrimSuffix("/");
        var ret = DirAccess.MakeDirRecursiveAbsolute(Folder);
        if (ret != Error.Ok)
        {
            throw new Exception($"Failed to create directory {Folder}");
        }
        ret = ResourceSaver.Save(UI, $"{Folder}/UI.tscn");
        if (ret != Error.Ok)
        {
            throw new Exception($"Failed to save UI");
        }
        ret = ResourceSaver.Save(Canvas, $"{Folder}/Canvas.tscn");
        if (ret != Error.Ok)
        {
            throw new Exception($"Failed to save Canvas");
        }

        using (
            var file =
                FileAccess.Open($"{Folder}/global.json", FileAccess.ModeFlags.Write)
                ?? throw new Exception($"Failed to create file {Folder}/global.json")
        )
        {
            file.StoreString(global);
        }
    }

    public AVGRuntime Instantiate()
    {
        throw new NotImplementedException();
    }
}
