namespace RingEngine.Runtime.Effect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using EffectGroup = System.Collections.Generic.Dictionary<Godot.Node, IEffect>;

public interface ITransition
{
    /// <summary>
    /// 在给定节点组上构建转场
    /// </summary>
    /// <returns>若干效果组，直接提交给EffectBuffer即可</returns>
    public IEnumerable<EffectGroup> Build(Canvas canvas, Texture2D newBG);

    /// <summary>
    /// 获取转场的持续时间
    /// </summary>
    public float GetDuration();
}

public class DissolveTrans : ITransition
{
    public Texture2D mask;
    public float duration;

    public DissolveTrans(Texture2D mask = null, double duration = 2)
    {
        this.mask = GD.Load<Texture2D>("res://assets/Runtime/black.png");
        this.duration = (float)duration;
    }

    public IEnumerable<EffectGroup> Build(Canvas canvas, Texture2D newBG)
    {
        var group1 = new EffectGroupBuilder()
            .Add(canvas.Mask, new Chain([
                new LambdaEffect(() =>
                {
                    canvas.AddMask(mask);
                    canvas.Mask.Modulate = new Color(canvas.Mask.Modulate, 0);
                }),
                new Dissolve(duration / 2),
            ]))
            .Build();
        var group2 = new EffectGroupBuilder()
            .Add(canvas.Mask, new Chain([
                new LambdaEffect(() => canvas["BG"].Texture = canvas.Stretch(newBG)),
                new Fade(duration / 2),
                new LambdaEffect(canvas.RemoveMask),
            ]))
            .Build();
        return new[] { group1, group2 }.AsEnumerable();
    }
    public float GetDuration() => duration;
}

public class ImageTrans : ITransition
{
    public Texture2D mask;
    public Texture2D control;
    public float duration;

    public ImageTrans(Texture2D mask = null, double duration = 2)
    {
        this.mask = GD.Load<Texture2D>("res://assets/Runtime/black.png");
        this.control = GD.Load<Texture2D>("res://assets/Runtime/rule_10.png");
        this.duration = (float)duration;
    }

    public IEnumerable<EffectGroup> Build(Canvas canvas, Texture2D newBG)
    {
        var group1 = new EffectGroupBuilder()
            .Add(canvas.Mask, new Chain([
                new LambdaEffect(() =>
                {
                    canvas.AddMask(mask);
                    var material = new ShaderMaterial
                    {
                        Shader = GD.Load<Shader>("res://Runtime/Effect/mask.gdshader"),
                    };
                    material.SetShaderParameter("progress", 0.0);
                    material.SetShaderParameter("smooth_size", 0.3);
                    material.SetShaderParameter("control", control);
                    canvas.Mask.Material = material;
                }),
                new LambdaEffect((Node node, Tween tween) =>
                {
                    tween.TweenMethod(Callable.From((float progress) =>
                    {
                        ((ShaderMaterial)canvas.Mask.Material).SetShaderParameter("progress", progress);
                    }), 0.0, 1.0, duration / 2);
                    return tween;
                })
            ]))
            .Build();
        var group2 = new EffectGroupBuilder()
            .Add(canvas.Mask, new Chain([
                new LambdaEffect(() => canvas["BG"].Texture = canvas.Stretch(newBG)),
                new LambdaEffect((Node node, Tween tween) =>
                {
                    tween.TweenMethod(Callable.From((float progress) =>
                    {
                        ((ShaderMaterial)canvas.Mask.Material).SetShaderParameter("progress", progress);
                    }), 1.0, 0.0, duration / 2);
                    return tween;
                }),
                new LambdaEffect(() => canvas.Mask.Material = null),
                new LambdaEffect(canvas.RemoveMask),
            ]))
            .Build();
        return new[] { group1, group2 }.AsEnumerable();
    }
    public float GetDuration() => duration;
}
