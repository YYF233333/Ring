﻿using Godot;
using RingEngine.Runtime;
using System;
using System.Collections.Generic;

public partial class UI : Control
{
    public delegate void EventHandler(object args);

    // 各组件注册的回调函数
    Dictionary<string, EventHandler> handlers = new Dictionary<string, EventHandler>();

    public string chapterName
    {
        get => GetNode<Label>("./ChapterNameBack/ChapterName").Text;
        set => GetNode<Label>("./ChapterNameBack/ChapterName").Text = value;
    }

    public string text
    {
        get => GetNode<RichTextLabel>("./TextBoxBack/MarginContainer/MarginContainer/TextBox").Text;
        set => GetNode<RichTextLabel>("./TextBoxBack/MarginContainer/MarginContainer/TextBox").Text = value;
    }

    public string characterName
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

    /// <summary>
    /// 将内容输出到主文本框
    /// </summary>
    public void Print(string content, bool append = false)
    {
        if (append)
        {
            text += content;
        }
        else
        {
            text = content;
        }
    }
}
