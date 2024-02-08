using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RingEngine.Runtime.Effect
{
    public interface IEffect
    {
        /// <summary>
        /// 对节点应用当前效果，动效的目标状态通常为节点的当前状态
        /// </summary>
        /// <param name="node">目标节点</param>
        /// 
        /// <param name="tween">Tween Object（如果已经创建）</param>
        public Tween Apply(Node node, Tween tween = null);

        /// <summary>
        /// 获取效果的持续时间
        /// </summary>
        /// <returns></returns>
        public float GetDuration();
    }

    // 临时定义效果函数
    public delegate Tween EffectFunc(Node node, Tween tween = null);

    public static class Effects
    {
        static Dictionary<string, IEffect> effects = new Dictionary<string, IEffect>
        {
            {"transparent", new SetAlpha(0) },
            {"opaque", new SetAlpha(1) },
            {"dissolve", new Dissolve() },
            {"fade", new Fade() }
        };

        public static IEffect Get(string name)
        {
            return effects[name];
        }
    }

    public class Chain : IEffect
    {
        List<IEffect> effects;

        public Chain(List<IEffect> effects)
        {
            this.effects = effects;
        }

        public Tween Apply(Node node, Tween tween = null)
        {
            tween ??= node.CreateTween();
            foreach (var effect in effects)
            {
                tween = effect.Apply(node, tween);
            }
            return tween;
        }

        public float GetDuration()
        {
            return effects.Sum(x => x.GetDuration());
        }
    }

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

        public float GetDuration()
        {
            return duration;
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
            var sprite = (CanvasItem)node;
            var startAlpha = this.startAlpha ?? sprite.Modulate.A;
            tween.TweenCallback(Callable.From(() =>
            {
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
