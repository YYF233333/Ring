namespace RingEngine.Runtime.Storage;

using System.Collections.Generic;
using MessagePack;

[MessagePackObject(keyAsPropertyName: true)]
public class DataBase
{
    /// <summary>
    /// 下一条执行的代码块index
    /// </summary>
    public int PC;

    public Dictionary<string, object> global = [];

    [IgnoreMember]
    public List<Snapshot> history = [];

    public Snapshot LoadHistory(int step)
    {
        var ret = history[^step];
        history.RemoveRange(history.Count - step, step);
        return ret;
    }

    public void SetGlobal(string name, object value)
    {
        global[name] = value;
    }

    public object GetGlobal(string name)
    {
        return global[name];
    }

    public string Serialize()
    {
        return MessagePackSerializer.SerializeToJson(this);
    }

    public static DataBase Deserialize(string json)
    {
        return MessagePackSerializer.Deserialize<DataBase>(MessagePackSerializer.ConvertFromJson(json));
    }
}
