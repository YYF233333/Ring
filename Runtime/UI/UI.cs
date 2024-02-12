using Godot;
using RingEngine.Runtime;
using RingEngine.Runtime.Effect;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class UI : Control
{
    public delegate void EventHandler(object args);

    // 各组件注册的回调函数
    Dictionary<string, EventHandler> handlers = new Dictionary<string, EventHandler>();

    public TextureRect ChapterNameBack => GetNode<TextureRect>("./ChapterNameBack");

    public string ChapterName
    {
        get => GetNode<Label>("./ChapterNameBack/ChapterName").Text;
        set => GetNode<Label>("./ChapterNameBack/ChapterName").Text = value;
    }

    public RichTextLabel TextBox => GetNode<RichTextLabel>("./TextBoxBack/MarginContainer/MarginContainer/TextBox");

    public string CharacterName
    {
        get => GetNode<RichTextLabel>("./TextBoxBack/MarginContainer/MarginContainer2/TextBox").Text.Trim(['【', '】']);
        set => GetNode<RichTextLabel>("./TextBoxBack/MarginContainer/MarginContainer2/TextBox").Text = '【' + value + '】';
    }

    public void RegisterCallback(string name, EventHandler handler) { handlers[name] = handler; }

    public void UnregisterCallback(string name) { handlers.Remove(name); }

    public void Call(string name, object args)
    {
        handlers[name](args);
    }
}
