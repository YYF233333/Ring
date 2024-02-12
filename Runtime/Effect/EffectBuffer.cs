using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EffectGroup = System.Collections.Generic.Dictionary<Godot.Node, RingEngine.Runtime.Effect.IEffect>;

namespace RingEngine.Runtime.Effect
{
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
        // 正在构建的效果组（用于块间同步）
        public EffectGroup constructingGroup = null;

        /// <summary>
        /// 创建一个空效果组，暂存于constructingGroup，可以向其中添加Effect
        /// </summary>
        public EffectGroup CreateOrGetGroup()
        {
            constructingGroup ??= new EffectGroup();
            return constructingGroup;
        }

        /// <summary>
        /// 结束构建效果组，将其加入等待队列
        /// </summary>
        public void FinishConstructGroup()
        {
            Trace.Assert(constructingGroup != null);
            buffer.Enqueue(constructingGroup);
            constructingGroup = null;
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
                if (tweens.ContainsKey(node))
                {
                    effect.Apply(node, tweens[node]);
                }
                else
                {
                    var tween = node.CreateTween();
                    tween.Finished += Decrement;
                    effect.Apply(node, tween);
                    tweens[node] = tween;
                }
            }
            return tweens.Values.ToHashSet();
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
}
