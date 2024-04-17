namespace RingEngine.Runtime.Effect;

using System;
using System.Diagnostics;
using Godot;

public class SetAlpha : IEffect
{
    public static SetAlpha Transparent = new(0);
    public static SetAlpha Opaque = new(1);

    public float alpha;

    public SetAlpha(double alpha)
    {
        this.alpha = (float)alpha;
    }

    public void Apply(Node node, Tween tween)
    {
        tween.TweenCallback(
            Callable.From(() =>
            {
                var sprite = (CanvasItem)node;
                var c = sprite.Modulate;
                c.A = alpha;
                sprite.Modulate = c;
            })
        );
    }

    public double GetDuration() => 0;
}

public class Delete : IEffect
{
    public void Apply(Node node, Tween tween)
    {
        tween.TweenCallback(
            Callable.From(() =>
            {
                var canvas = node.GetParent<Canvas>();
                canvas.RemoveTexture(node.Name);
            })
        );
    }

    public double GetDuration() => 0;
}

public class Delay : IEffect
{
    public double duration;

    public Delay(double duration)
    {
        this.duration = duration;
    }

    public void Apply(Node node, Tween tween)
    {
        tween.TweenInterval(duration);
    }

    public double GetDuration() => duration;
}

public class Dissolve : IEffect
{
    public float endAlpha;
    public double duration;

    public Dissolve(double duration = 1.0, double endAlpha = 1.0)
    {
        this.endAlpha = (float)endAlpha;
        this.duration = duration;
    }

    public void Apply(Node node, Tween tween)
    {
        Trace.Assert(node.IsClass("CanvasItem"));
        tween.TweenProperty(node, "modulate:a", endAlpha, duration);
    }

    public override bool Equals(object obj)
    {
        return obj is Dissolve dissolve
            && endAlpha == dissolve.endAlpha
            && duration == dissolve.duration;
    }

    public double GetDuration() => duration;

    public override int GetHashCode() => HashCode.Combine(endAlpha, duration);
}

public class Fade : IEffect
{
    public float endAlpha;
    public double duration;

    public Fade(double duration = 1.0, double endAlpha = 0.0)
    {
        this.endAlpha = (float)endAlpha;
        this.duration = duration;
    }

    public void Apply(Node node, Tween tween)
    {
        Trace.Assert(node.IsClass("CanvasItem"));
        tween.TweenProperty(node, "modulate:a", endAlpha, duration);
    }

    public double GetDuration() => duration;
}
