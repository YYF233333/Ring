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

    public Tween Apply(Node node, Tween tween)
    {
        tween.TweenCallback(Callable.From(() =>
        {
            var sprite = (CanvasItem)node;
            var c = sprite.Modulate;
            c.A = alpha;
            sprite.Modulate = c;
        }));
        return tween;
    }

    public float GetDuration() => 0;
}

public class Delete : IEffect
{
    public Tween Apply(Node node, Tween tween)
    {
        tween.TweenCallback(Callable.From(() =>
        {
            var canvas = node.GetParent<Canvas>();
            canvas.RemoveTexture(node.Name);
        }));
        return tween;
    }

    public float GetDuration() => 0;
}

public class Delay : IEffect
{
    public float duration;

    public Delay(double duration)
    {
        this.duration = (float)duration;
    }

    public Tween Apply(Node node, Tween tween)
    {
        tween.TweenInterval(duration);
        return tween;
    }

    public float GetDuration() => duration;
}

public class Dissolve : IEffect
{
    public float endAlpha;
    public float duration;
    public Dissolve(double duration = 1.0, double endAlpha = 1.0)
    {
        this.endAlpha = (float)endAlpha;
        this.duration = (float)duration;
    }
    public Tween Apply(Node node, Tween tween)
    {
        Trace.Assert(node.IsClass("CanvasItem"));
        tween.TweenProperty(node, "modulate:a", endAlpha, duration);
        return tween;
    }

    public override bool Equals(object obj)
    {
        return obj is Dissolve dissolve &&
               endAlpha == dissolve.endAlpha &&
               duration == dissolve.duration;
    }

    public float GetDuration() => duration;

    public override int GetHashCode() => HashCode.Combine(endAlpha, duration);
}

public class Fade : IEffect
{
    public float endAlpha;
    public float duration;
    public Fade(double duration = 1.0, double endAlpha = 0.0)
    {
        this.endAlpha = (float)endAlpha;
        this.duration = (float)duration;
    }
    public Tween Apply(Node node, Tween tween)
    {
        tween ??= node.CreateTween();
        Trace.Assert(node.IsClass("CanvasItem"));
        tween.TweenProperty(node, "modulate:a", endAlpha, duration);
        return tween;
    }

    public float GetDuration() => duration;
}
