namespace RingEngine.Runtime.Storage;

using System.Collections.Generic;
using System.Text.Json;
using Godot;

public class DataBase
{
    /// <summary>
    /// 下一条执行的代码块index
    /// </summary>
    public int PC;

    public List<Snapshot> history = [];

    public Snapshot LoadHistory(int step)
    {
        var ret = history[^step];
        history.RemoveRange(history.Count - step, step);
        return ret;
    }

    public string Serialize()
    {
        return JsonSerializer.Serialize(
            new Dictionary<string, object>
            {
                { "PC", PC }
            });
    }

    public void Deserialize(string json)
    {
        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        PC = ((JsonElement)dict["PC"]).GetInt32();
    }
}
