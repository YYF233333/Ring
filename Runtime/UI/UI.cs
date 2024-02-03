using Godot;
using RingEngine.Runtime;
using System;
using System.Collections.Generic;

public partial class UI : Control
{
    public delegate void EventHandler(object args);

    // 各组件注册的回调函数
    Dictionary<string, EventHandler> handlers = new Dictionary<string, EventHandler>();

    public void RegisterCallback(string name, EventHandler handler)
    {
        handlers[name] = handler;
    }

    public void UnregisterCallback(string name) { handlers.Remove(name); }

    public void Call(string name, object args)
    {
        handlers[name](args);
    }

    /// <summary>
    /// 将内容输出到主文本框
    /// </summary>
    public void Print(string content, bool append = false)
    {
        GD.Print(content);
    }

    /// <summary>
    /// 替换人物名称文本框内容
    /// </summary>
    public void ChangeCharacterName(string name)
    {
        GD.Print($"Character: {name}");
    }
}
