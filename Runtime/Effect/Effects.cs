using Godot;
using System;

namespace RingEngine.Runtime.Effect
{
    public class SetAlpha : IEffect
    {
        public static SetAlpha Transparent = new SetAlpha(0);
        public static SetAlpha Opaque = new SetAlpha(1);

        public float alpha;

        public SetAlpha(double alpha)
        {
            this.alpha = (float)alpha;
        }

        public Tween Apply(Node node, Tween tween = null)
        {
            tween ??= node.CreateTween();
            tween.TweenCallback(Callable.From(() =>
            {
                var sprite = (CanvasItem)node;
                Color c = sprite.Modulate;
                c.A = alpha;
                sprite.Modulate = c;
            }));
            return tween;
        }

        public float GetDuration()
        {
            return 0;
        }
    }

    public class Delete : IEffect
    {
        public Tween Apply(Node node, Tween tween = null)
        {
            tween ??= node.CreateTween();
            tween.TweenCallback(Callable.From(() =>
            {
                var canvas = node.GetParent<Canvas>();
                canvas.RemoveTexture(node.Name);
            }));
            return tween;
        }

        public float GetDuration()
        {
            return 0;
        }
    }

    public class Delay : IEffect
    {
        public float duration;

        public Delay(double duration)
        {
            this.duration = (float)duration;
        }

        public Tween Apply(Node node, Tween tween = null)
        {
            tween ??= node.CreateTween();
            tween.TweenInterval(duration);
            return tween;
        }

        public float GetDuration()
        {
            return duration;
        }
    }

    public class Dissolve : IEffect
    {
        public float startAlpha;
        public float? endAlpha;
        public float duration;
        public Dissolve(double duration = 1.0, double startAlpha = 0.0, double? endAlpha = null)
        {
            this.startAlpha = (float)startAlpha;
            this.endAlpha = (float?)endAlpha;
            this.duration = (float)duration;
        }
        public Tween Apply(Node node, Tween tween = null)
        {
            tween ??= node.CreateTween();
            var sprite = (CanvasItem)node;
            var endAlpha = this.endAlpha ?? sprite.Modulate.A;
            tween.TweenCallback(Callable.From(() =>
            {
                Color c = sprite.Modulate;
                c.A = startAlpha;
                sprite.Modulate = c;
            }));
            tween.TweenProperty(node, "modulate:a", endAlpha, duration);
            return tween;
        }

        public override bool Equals(object obj)
        {
            return obj is Dissolve dissolve &&
                   startAlpha == dissolve.startAlpha &&
                   endAlpha == dissolve.endAlpha &&
                   duration == dissolve.duration;
        }

        public float GetDuration()
        {
            return duration;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(startAlpha, endAlpha, duration);
        }
    }

    public class Fade : IEffect
    {
        public float? startAlpha;
        public float endAlpha;
        public float duration;
        public Fade(double duration = 1.0, double endAlpha = 0.0, double? startAlpha = null)
        {
            this.startAlpha = (float?)startAlpha;
            this.endAlpha = (float)endAlpha;
            this.duration = (float)duration;
        }
        public Tween Apply(Node node, Tween tween = null)
        {
            tween ??= node.CreateTween();
            tween.TweenCallback(Callable.From(() =>
            {
                var sprite = (CanvasItem)node;
                var startAlpha = this.startAlpha ?? sprite.Modulate.A;
                Color c = sprite.Modulate;
                c.A = startAlpha;
                sprite.Modulate = c;
            }));
            tween.TweenProperty(node, "modulate:a", endAlpha, duration);
            return tween;
        }

        public float GetDuration()
        {
            return duration;
        }
    }
}
