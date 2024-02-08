using Godot;
using System;
using System.Collections.Generic;
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
        public void Apply(Node node);

        /// <summary>
        /// 获取效果的持续时间
        /// </summary>
        /// <returns></returns>
        public float getDuration();
    }

    // 临时定义效果函数
    public delegate void EffectFunc(Node node);

    public static class Effects
    {

        static Dictionary<string, IEffect> effects = new Dictionary<string, IEffect>
        {
            {"transparent", new SetAlpha(0) },
            {"opaque", new SetAlpha(1) },
            {"dissolve", new Dissolve() }
        };

        public static IEffect Get(string name)
        {
            return effects[name];
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

        public void Apply(Node node)
        {
            var sprite = (Sprite2D)node;
            Color c = sprite.Modulate;
            c.A = alpha;
            sprite.Modulate = c;
        }

        public float getDuration()
        {
            return 0;
        }
    }

    public class Defer : IEffect
    {
        public float duration;
        public EffectFunc effect;

        public Defer(EffectFunc effect, float duration)
        {
            this.duration = duration;
            this.effect = effect;
        }

        public void Apply(Node node)
        {
            var tween = node.CreateTween();
            tween.TweenCallback(Callable.From(() => effect(node))).SetDelay(duration);
        }

        public float getDuration()
        {
            throw new NotImplementedException();
        }
    }

    public class Dissolve : IEffect
    {
        public float startAlpha;
        public float endAlpha;
        public float duration;
        public Dissolve(double duration = 1.0, double endAlpha = 1.0, double startAlpha = 0.0)
        {
            this.startAlpha = (float)startAlpha;
            this.endAlpha = (float)endAlpha;
            this.duration = (float)duration;
        }
        public void Apply(Node node)
        {
            var sprite = (Sprite2D)node;
            Color c = sprite.Modulate;
            c.A = startAlpha;
            sprite.Modulate = c;
            var tween = node.CreateTween();
            tween.TweenProperty(node, "modulate:a", endAlpha, duration);
        }

        public float getDuration()
        {
            return duration;
        }
    }

    public class Fade : IEffect
    {
        public float startAlpha;
        public float endAlpha;
        public float duration;

        public void Apply(Node node)
        {
            throw new NotImplementedException();
        }

        public float getDuration()
        {
            throw new NotImplementedException();
        }
    }
}
