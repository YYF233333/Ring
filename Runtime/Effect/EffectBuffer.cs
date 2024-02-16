namespace RingEngine.Runtime.Effect;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using EffectGroup = System.Collections.Generic.Dictionary<Godot.Node, IEffect>;

public class EffectGroupBuilder
{
    EffectGroup group = [];

    public EffectGroupBuilder Add(Node node, IEffect effect)
    {
        group[node] = effect;
        return this;
    }

    public EffectGroup Build() => group;
}

public class EffectBuffer
{
    // 当前正在运行的效果组
    HashSet<Tween> runningGroup = null;
    // 正在运行的Tween数量
    int activeTweenCount = 0;
    // 等待队列
    Queue<EffectGroup> buffer = [];
    // 当前是否有效果正在运行
    public bool IsRunning => activeTweenCount > 0;

    public void Append(EffectGroup group)
    {
        buffer.Enqueue(group);
        if (runningGroup == null)
        {
            Submit();
        }
    }

    public void Append(IEnumerable<EffectGroup> groups)
    {
        foreach (var group in groups)
        {
            buffer.Enqueue(group);
        }
        if (runningGroup == null)
        {
            Submit();
        }
    }

    public void Interrupt()
    {
        // 最后一个active Tween finish的时候就会触发Submit，提前解绑runningGroup防止Assert失败
        var group = runningGroup;
        runningGroup = null;
        foreach (var tween in group)
        {
            if (tween.IsRunning())
            {
                tween.Pause();
                tween.CustomStep(114514);
                tween.Kill();
            }
        }
    }

    private void Submit()
    {
        Trace.Assert(runningGroup == null);
        if (buffer.Count > 0)
        {
            runningGroup = Initialize(buffer.Dequeue());
            activeTweenCount = runningGroup.Count;
        }
    }

    private HashSet<Tween> Initialize(EffectGroup group)
    {
        Dictionary<Node, Tween> tweens = [];
        foreach (var (node, effect) in group)
        {
            if (tweens.TryGetValue(node, out var value))
            {
                effect.Apply(node, value);
            }
            else
            {
                var tween = node.CreateTween();
                tween.Finished += Decrement;
                effect.Apply(node, tween);
                tweens[node] = tween;
            }
        }
        return [.. tweens.Values];
    }

    private void Decrement()
    {
        activeTweenCount--;
        Trace.Assert(activeTweenCount >= 0);
        if (activeTweenCount == 0)
        {
            runningGroup = null;
            Submit();
        }
    }
}
