using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void apply(Node node);
    }

    public static class Effects
    {
        static Dictionary<string, IEffect> effects = new Dictionary<string, IEffect>
        {
            {"dissolve", new Dissolve() }
        };

        public static IEffect Get(string name)
        {
            return effects[name];
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
        public void apply(Node node)
        {
            var sprite = (Sprite2D)node;
            Color c = sprite.Modulate;
            c.A = startAlpha;
            sprite.Modulate = c;
            var tween = node.CreateTween();
            tween.TweenProperty(node, "modulate:a", endAlpha, duration);
        }
    }
}
