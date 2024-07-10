namespace RingEngine.Runtime.Storage;

using System;
using System.Collections.Generic;
using Godot;
using MessagePack;

[MessagePackObject(keyAsPropertyName: true)]
public class DataBase
{
    /// <summary>
    /// 下一条执行的代码块index
    /// </summary>
    public int PC { get; set; }

    /// <summary>
    /// 当前是否有脚本语句在执行
    /// </summary>
    public bool IsExecuting { get; set; }

    /// <summary>
    /// 上一个选择支选择的选项ID
    /// </summary>
    public int LastChosenOptionId { get; set; }

    /// <summary>
    /// 其它数据，为了使python可调用需要限制类型
    /// </summary>
    public Dictionary<string, string> Data = new() { { "test", "aaa" } };

    public string this[string key]
    {
        get => Data[key];
        set => Data[key] = value;
    }

    [IgnoreMember]
    public List<Snapshot> History = [];

    public Snapshot LoadHistory(int step)
    {
        if (step < 1 || step > History.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(step));
        }
        var ret = History[^step];
        History.RemoveRange(History.Count - step, step);
        return ret;
    }

    public string Serialize()
    {
        return MessagePackSerializer.SerializeToJson(this);
    }

    public static DataBase Deserialize(string json)
    {
        return MessagePackSerializer.Deserialize<DataBase>(
            MessagePackSerializer.ConvertFromJson(json)
        );
    }
}
