namespace RingEngine.Runtime.Effect;
using System.Collections.Generic;
using System.Linq;
using Godot;

public interface IEffect
{
    /// <summary>
    /// 对节点应用当前效果
    /// </summary>
    /// <param name="node">目标节点</param>
    /// 
    /// <param name="tween">Tween Object（如果已经创建）</param>
    public Tween Apply(Node node, Tween tween);

    /// <summary>
    /// 获取效果的持续时间
    /// </summary>
    public float GetDuration();
}

// 用于参数类型
public delegate Tween EffectFunc(Node node, Tween tween);

public static class Effects
{
    public static Dictionary<string, IEffect> effects = new()
    {
        {"transparent", new SetAlpha(0) },
        {"opaque", new SetAlpha(1) },
        {"dissolve", new Dissolve() },
        {"fade", new Fade() }
    };
}

public class LambdaEffect : IEffect
{
    EffectFunc func;
    float duration;

    public LambdaEffect(EffectFunc func, double duration = 0)
    {
        this.func = func;
        this.duration = (float)duration;
    }

    public Tween Apply(Node node, Tween tween)
    {
        tween ??= node.CreateTween();
        return func(node, tween);
    }

    public float GetDuration() => duration;
}

public class Chain : IEffect
{
    List<IEffect> effects;

    public Chain(List<IEffect> effects)
    {
        this.effects = effects;
    }

    public Tween Apply(Node node, Tween tween)
    {
        tween ??= node.CreateTween();
        foreach (var effect in effects)
        {
            tween = effect.Apply(node, tween);
        }
        return tween;
    }

    public float GetDuration() => effects.Sum(x => x.GetDuration());
}
