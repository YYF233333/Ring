namespace RingEngine.Runtime.Effect;
using System.Collections.Generic;
using System.Linq;
using Godot;

public interface IEffect
{
    /// <summary>
    /// 对节点应用当前效果
    /// </summary>
    public void Apply(Node node, Tween tween);

    /// <summary>
    /// 获取效果的持续时间
    /// </summary>
    public double GetDuration();
}

// 用于参数类型
public delegate void EffectFunc(Node node, Tween tween);

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
    public delegate void CallBack();
    EffectFunc func;
    double duration;

    public LambdaEffect(EffectFunc func, double duration = 0)
    {
        this.func = func;
        this.duration = duration;
    }

    /// <summary>
    /// 仅包含单个TweenCallBack的LambdaEffect
    /// </summary>
    /// <param name="callBack">要调用的CallBack</param>
    /// <param name="duration">效果持续时间</param>
    public LambdaEffect(CallBack callBack, double duration = 0)
    {
        this.func = (_, tween) => tween.TweenCallback(Callable.From(() => callBack()));
        this.duration = duration;
    }

    public void Apply(Node node, Tween tween)
    {
        func(node, tween);
    }

    public double GetDuration() => duration;
}

public class Chain : IEffect
{
    IEffect[] effects;

    public Chain(params IEffect[] effects)
    {
        this.effects = effects;
    }

    public void Apply(Node node, Tween tween)
    {
        foreach (var effect in effects)
        {
            effect.Apply(node, tween);
        }
    }

    public double GetDuration() => effects.Sum(x => x.GetDuration());
}
