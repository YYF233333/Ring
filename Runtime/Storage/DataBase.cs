namespace RingEngine.Runtime.Storage;

using System.Collections.Generic;
using System.Text.Json;
using Godot;

public class DataBase
{
    /// <summary>
    /// 下一条执行的代码块index
    /// </summary>
    public int PC { get => (int)data["PC"]; set => data["PC"] = value; }

    public Dictionary<string, int> data = new()
    {
        {"PC", 0 }
    };

    public List<Snapshot> history = [];

    public int this[string key]
    {
        get => data[key];
        set => data[key] = value;
    }

    public string Serialize() => JsonSerializer.Serialize(data);

    public void Deserialize(string json)
    {
        data = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
    }
}
