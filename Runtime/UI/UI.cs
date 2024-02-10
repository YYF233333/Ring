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

    public Dictionary<Node, Tween> tweens { get => GetParent<Runtime>().tweens; }

    public string chapterName
    {
        get => GetNode<Label>("./ChapterNameBack/ChapterName").Text;
        set => GetNode<Label>("./ChapterNameBack/ChapterName").Text = value;
    }

    public RichTextLabel textBox
    {
        get => GetNode<RichTextLabel>("./TextBoxBack/MarginContainer/MarginContainer/TextBox");
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
            textBox.Text += content;
        }
        else
        {
            textBox.Text = content;
            Trace.Assert(false == tweens.ContainsKey(textBox));
            var textBoxTween = textBox.CreateTween();
            tweens[textBox] = textBoxTween;
            textBox.VisibleRatio = 0;
            textBoxTween.TweenProperty(textBox, "visible_ratio", 1.0, 1.0);

        }
    }

    public void ShowChapterName(string chapterName, IEffect present = null, IEffect disappear = null, double duration = 2.0)
    {
        Trace.Assert(false == tweens.ContainsKey(GetNode("ChapterNameBack")));
        present ??= new Dissolve(endAlpha: 1.0);
        disappear ??= new Fade();
        var effect = new Chain([present, new Delay(duration), disappear]);
        this.chapterName = chapterName;
        var tween = effect.Apply(GetNode("ChapterNameBack"));
        tweens[GetNode("ChapterNameBack")] = tween;
    }
}
