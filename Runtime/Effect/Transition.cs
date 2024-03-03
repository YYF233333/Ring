namespace RingEngine.Runtime.Effect;
using System.Collections.Generic;
using System.Linq;
using Godot;
using EffectGroup = System.Collections.Generic.Dictionary<Godot.Node, IEffect>;

/// <summary>
/// 转场效果，对整个<c>Canvas</c>应用的一个复杂变换，可以包含多组<c>IEffect</c>。
/// </summary>
public interface ITransition
{
    /// <summary>
    /// 在当前<c>Canvas</c>上构建转场效果
    /// </summary>
    /// <param name="runtime"></param>
    /// <param name="newBG"></param>
    /// <returns>若干效果组，直接提交给EffectBuffer即可</returns>
    public IEnumerable<EffectGroup> Build(Runtime runtime, Texture2D newBG);

    /// <summary>
    /// 获取转场的持续时间
    /// </summary>
    public double GetDuration();
}

public class DissolveTrans : ITransition
{
    public Texture2D mask;
    public double duration;

    public DissolveTrans(Texture2D mask = null, double duration = 2)
    {
        this.mask = GD.Load<Texture2D>("res://assets/Runtime/black.png");
        this.duration = duration;
    }

    public IEnumerable<EffectGroup> Build(Runtime runtime, Texture2D newBG)
    {
        var canvas = runtime.canvas;
        var group1 = new EffectGroupBuilder()
            .Add(canvas.Mask, new Chain(
                new LambdaEffect(() =>
                {
                    canvas.AddMask(mask);
                    canvas.Mask.Modulate = new Color(canvas.Mask.Modulate, 0);
                }),
                new Dissolve(duration / 2)
            ))
            .Build();
        var group2 = new EffectGroupBuilder()
            .Add(canvas.Mask, new Chain(
                new LambdaEffect(() => canvas["BG"].Texture = canvas.Stretch(newBG)),
                new Fade(duration / 2),
                new LambdaEffect(canvas.RemoveMask)
            ))
            .Build();
        return new[] { group1, group2 }.AsEnumerable();
    }
    public double GetDuration() => duration;
}

public class ImageTrans : ITransition
{
    public string maskPath = "res://assets/Runtime/black.png";
    public string controlPath;
    public double duration;
    public bool reversed;
    public float smooth;

    public ImageTrans(double duration = 2, bool reversed = false, double smooth = 0.2)
    {
        this.controlPath = "res://assets/Runtime/rule_10.png";
        this.duration = duration;
        this.reversed = reversed;
        this.smooth = (float)smooth;
    }

    public ImageTrans(string controlPath, double duration = 2, bool reversed = false, double smooth = 0.2)
    {
        this.controlPath = controlPath;
        this.duration = duration;
        this.reversed = reversed;
        this.smooth = (float)smooth;
    }

    public IEnumerable<EffectGroup> Build(Runtime runtime, Texture2D newBG)
    {
        if (!maskPath.StartsWith("res://"))
        {
            maskPath = runtime.script.ToResourcePath(maskPath);
        }
        if (!controlPath.StartsWith("res://"))
        {
            controlPath = runtime.script.ToResourcePath(controlPath);
        }
        var mask = GD.Load<Texture2D>(maskPath);
        var control = GD.Load<Texture2D>(controlPath);
        var canvas = runtime.canvas;
        var group1 = new EffectGroupBuilder()
            .Add(canvas.Mask, new Chain(
                new LambdaEffect(() =>
                {
                    canvas.AddMask(mask);
                    var material = new ShaderMaterial
                    {
                        Shader = GD.Load<Shader>("res://Runtime/Effect/mask.gdshader"),
                    };
                    material.SetShaderParameter("progress", 0.0);
                    material.SetShaderParameter("smooth_size", smooth);
                    material.SetShaderParameter("control", control);
                    material.SetShaderParameter("reversed", reversed);
                    canvas.Mask.Material = material;
                }),
                new LambdaEffect((Node node, Tween tween) =>
                {
                    tween.TweenMethod(Callable.From((float progress) =>
                    {
                        ((ShaderMaterial)canvas.Mask.Material).SetShaderParameter("progress", progress);
                    }), 0.0, 1.0, duration / 2);
                })
            ))
            .Build();
        var group2 = new EffectGroupBuilder()
            .Add(canvas.Mask, new Chain(
                new LambdaEffect(() => canvas["BG"].Texture = canvas.Stretch(newBG)),
                new LambdaEffect((Node node, Tween tween) =>
                {
                    tween.TweenMethod(Callable.From((float progress) =>
                    {
                        ((ShaderMaterial)canvas.Mask.Material).SetShaderParameter("progress", progress);
                    }), 1.0, 0.0, duration / 2);
                }),
                new LambdaEffect(() => canvas.Mask.Material = null),
                new LambdaEffect(canvas.RemoveMask)
            ))
            .Build();
        return new[] { group1, group2 }.AsEnumerable();
    }
    public double GetDuration() => duration;
}
