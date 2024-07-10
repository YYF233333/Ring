namespace RingEngine.Runtime.AVGRuntime.Script;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Godot;
using Python.Runtime;
using RingEngine.Runtime.AVGRuntime;
using RingEngine.Runtime.AVGRuntime.Effect;

public class RingScript
{
    // 脚本所在文件夹路径
    public string folderPath;

    // 脚本文件名
    public string scriptName;
    public List<IScriptBlock> segments;
    public Dictionary<string, int> labels;

    public RingScript(string filePath)
    {
        Trace.Assert(filePath.StartsWith("res://"));
        folderPath = filePath.GetBaseDir();
        scriptName = Path.GetFileNameWithoutExtension(filePath);
        try
        {
            (segments, labels) = Parser.Parse(Godot.FileAccess.GetFileAsString(filePath));
        }
        catch (Exception ex)
        {
            GD.PrintErr(ex.Message);
        }
    }

    public string ToResourcePath(string filePath)
    {
        return Path.Combine(folderPath, filePath);
    }
}

public abstract class IScriptBlock
{
    /// <summary>
    /// 执行完当前语句块后是否继续执行
    /// </summary>
    public bool @continue = false;

    /// <summary>
    /// 执行当前代码块
    /// </summary>
    public abstract void Execute(AVGRuntime runtime);
}

public class CodeBlock : IScriptBlock
{
    // language specified in markdown codeblock(unused)
    public string identifier;
    public string code;

    public CodeBlock(string identifier, string code)
    {
        this.@continue = true;
        this.code = code;
        this.identifier = identifier;
    }

    // 测试代码使用
    public override bool Equals(object obj)
    {
        return obj is CodeBlock block && identifier == block.identifier && code == block.code;
    }

    public override void Execute(AVGRuntime runtime)
    {
        try
        {
            runtime.interpreter.Exec(code);
        }
        catch (PythonException ex)
        {
            GD.PrintErr(ex.Message);
        }
    }

    // 测试代码使用
    public override int GetHashCode() => HashCode.Combine(identifier, code);

    public override string ToString() => $"CodeBlock: identifier: {identifier}, code: {code}";
}

public class Branch : IScriptBlock
{
    public enum BranchType
    {
        Vertical,
        Horizontal
    }

    public BranchType Type;
    public string[] Options;

    public Branch(string[] table)
    {
        // table不能为空
        Trace.Assert(table.Length >= 3);
        Type = table[0] switch
        {
            "vertical" or "竖排" => BranchType.Vertical,
            "horizontal" or "横排" => BranchType.Horizontal,
            _ => throw new ArgumentException("Invalid branch type"),
        };
        Options = table[2..];
    }

    public override void Execute(AVGRuntime runtime)
    {
        switch (Type)
        {
            case BranchType.Vertical:
                runtime.VerticalBranch(Options);
                break;
            case BranchType.Horizontal:
                runtime.HorizontalBranch(Options);
                break;
            default:
                throw new UnreachableException();
        }
    }

    public override bool Equals(object obj) =>
        obj is Branch branch
        && this.@continue == branch.@continue
        && this.Type == branch.Type
        && EqualityComparer<string[]>.Default.Equals(this.Options, branch.Options);

    public override int GetHashCode() => HashCode.Combine(this.@continue, this.Type, this.Options);
}

public class JumpToLabel : IScriptBlock
{
    /// <summary>
    /// true => identifier为label名
    /// false => identifier为Lua表达式
    /// </summary>
    public bool isLiteral;
    public string identifier;

    public JumpToLabel(bool isLiteral, string identifier)
    {
        @continue = true;
        this.isLiteral = isLiteral;
        this.identifier = identifier;
    }

    public override bool Equals(object obj)
    {
        return obj is JumpToLabel label
            && this.@continue == label.@continue
            && this.isLiteral == label.isLiteral
            && this.identifier == label.identifier;
    }

    public override void Execute(AVGRuntime runtime)
    {
        var label = isLiteral ? identifier : runtime.interpreter.Eval(identifier);
        var targetPC = runtime.script.labels[label];
        // Execute结束后会PC++，所以这里要减1
        runtime.Global.PC = targetPC - 1;
    }

    public override int GetHashCode() =>
        HashCode.Combine(this.@continue, this.isLiteral, this.identifier);
}

public class Show : IScriptBlock
{
    public string imgName;
    public string imgPath;
    public string placement;
    public string effect;

    public Show(string path, string placement, string effect, string name)
    {
        @continue = true;
        imgName = name;
        imgPath = path;
        this.placement = placement;
        this.effect = effect;
    }

    public override bool Equals(object obj)
    {
        return obj is Show show
            && imgName == show.imgName
            && imgPath == show.imgPath
            && placement == show.placement
            && effect == show.effect;
    }

    public override void Execute(AVGRuntime runtime)
    {
        var texture = GD.Load<Texture2D>(Path.Combine(runtime.script.folderPath, imgPath));
        var windowSize = new Vector2I(950, 2184);
        texture = runtime.canvas.Stretch(texture, windowSize);
        var canvas = runtime.canvas;
        if (canvas.HasChild(imgName))
        { // 同名替换
            if (effect == "")
            {
                canvas.RemoveTexture(imgName);
                canvas.AddTexture(imgName, texture, runtime.interpreter.Eval<Placement>(placement));
            }
            else
            {
                canvas.RenameTexture(imgName, imgName + "_old");
                canvas.AddTexture(imgName, texture, runtime.interpreter.Eval<Placement>(placement));
                canvas[imgName].Modulate = new Color(canvas[imgName].Modulate, 0);
                IEffect instance = runtime.interpreter.Eval(effect);
                runtime.mainBuffer.Append(
                    new EffectGroupBuilder()
                        .Add(canvas[imgName], instance)
                        .Add(
                            canvas[imgName + "_old"],
                            new Chain(new Delay(instance.GetDuration()), new Delete())
                        )
                        .Build()
                );
            }
        }
        else
        {
            canvas.AddTexture(imgName, texture, runtime.interpreter.Eval<Placement>(placement));
            if (effect != "")
            {
                canvas[imgName].Modulate = new Color(canvas[imgName].Modulate, 0);
                IEffect instance = runtime.interpreter.Eval(effect);
                runtime.mainBuffer.Append(
                    new EffectGroupBuilder().Add(canvas[imgName], instance).Build()
                );
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(imgName, imgPath, placement, effect);

    public override string ToString() =>
        $"show: name: {imgName}, path: {imgPath}, placement: {placement}, effect: {effect}";
}

public class Hide : IScriptBlock
{
    public string name;
    public string effect;

    public Hide(string name, string effect)
    {
        this.name = name;
        this.effect = effect;
    }

    public override bool Equals(object obj)
    {
        return obj is Hide hide && name == hide.name && effect == hide.effect;
    }

    public override void Execute(AVGRuntime runtime)
    {
        runtime.mainBuffer.Append(
            new EffectGroupBuilder()
                .Add(
                    runtime.canvas[name],
                    new Chain(runtime.interpreter.Eval<IEffect>(effect), new Delete())
                )
                .Build()
        );
    }

    public override int GetHashCode() => HashCode.Combine(name, effect);

    public override string ToString() => $"hide: name: {name}, effect: {effect}";
}

public class ChangeBG : IScriptBlock
{
    public string imgPath;
    public string effect;

    public ChangeBG(string path, string effect)
    {
        @continue = true;
        this.imgPath = path;
        this.effect = effect;
    }

    public override bool Equals(object obj)
    {
        return obj is ChangeBG bG && imgPath == bG.imgPath && effect == bG.effect;
    }

    public override void Execute(AVGRuntime runtime)
    {
        var canvas = runtime.canvas;
        var texture = canvas.Stretch(
            GD.Load<Texture2D>(Path.Combine(runtime.script.folderPath, imgPath))
        );
        if (effect != "")
        {
            IEffect instance = runtime.interpreter.Eval(effect);
            // 对快进来说这里是checkpoint，正常运行时该group和后面的一起提交会连续运行
            runtime.mainBuffer.Append(
                new EffectGroupBuilder()
                    .Add(
                        runtime.canvas,
                        new LambdaEffect(
                            (_, tween) =>
                            {
                                var oldBG = canvas.ReplaceBG(texture);
                                canvas.BG.Modulate = new Color(canvas.BG.Modulate, 0);
                                new Chain(
                                    instance,
                                    new LambdaEffect(() =>
                                    {
                                        canvas.RemoveChild(oldBG);
                                        oldBG.QueueFree();
                                    })
                                ).Apply(canvas.BG, tween);
                            }
                        )
                    )
                    .Build()
            );
        }
        else
        {
            var oldBG = canvas.ReplaceBG(texture);
            canvas.RemoveChild(oldBG);
            oldBG.QueueFree();
        }
    }

    public override int GetHashCode() => HashCode.Combine(imgPath, effect);

    public override string ToString() => $"changeBG: path: {imgPath}, effect: {effect}";
}

public class ChangeScene : IScriptBlock
{
    public string bgPath;
    public string effect;

    public ChangeScene(string path, string effect)
    {
        //@continue = true;
        this.bgPath = path;
        this.effect = effect;
    }

    public override void Execute(AVGRuntime runtime)
    {
        var canvas = runtime.canvas;
        var texture = canvas.Stretch(
            GD.Load<Texture2D>(Path.Combine(runtime.script.folderPath, bgPath))
        );
        if (effect != "")
        {
            ITransition instance = runtime.interpreter.Eval(effect);
            // 对快进来说这里是checkpoint，正常运行时该group和后面的一起提交会连续运行
            runtime.mainBuffer.Append(instance.Build(runtime, texture));
        }
        else
        {
            var oldBG = canvas.ReplaceBG(texture);
            canvas.RemoveChild(oldBG);
            oldBG.QueueFree();
        }
    }
}

public class UIAnim : IScriptBlock
{
    public string effect;

    public UIAnim(string effect)
    {
        @continue = true;
        this.effect = effect;
    }

    public override bool Equals(object obj)
    {
        return obj is UIAnim anim && this.@continue == anim.@continue && this.effect == anim.effect;
    }

    public override void Execute(AVGRuntime runtime)
    {
        runtime.mainBuffer.Append(
            new EffectGroupBuilder()
                .Add(runtime.UI, runtime.interpreter.Eval<IEffect>(effect))
                .Build()
        );
    }

    public override int GetHashCode() => HashCode.Combine(this.@continue, this.effect);
}

public class ShowChapterName : IScriptBlock
{
    public string ChapterName;

    public ShowChapterName(string ChapterName)
    {
        @continue = true;
        this.ChapterName = ChapterName;
    }

    public override bool Equals(object obj)
    {
        return obj is ShowChapterName name && ChapterName == name.ChapterName;
    }

    public override void Execute(AVGRuntime runtime)
    {
        var init = new LambdaEffect(() =>
        {
            runtime.UI.ChapterName = ChapterName;
            var ChapterNameBack = runtime.UI.ChapterNameBack;
            ChapterNameBack.Modulate = new Color(ChapterNameBack.Modulate, 0);
        });
        runtime.nonBlockingBuffer.Append(
            new EffectGroupBuilder()
                .Add(runtime.UI.ChapterNameBack, new Chain(init, new Dissolve()))
                .Build()
        );
        runtime.nonBlockingBuffer.Append(
            new EffectGroupBuilder().Add(runtime.UI.ChapterNameBack, new Delay(2.0)).Build()
        );
        runtime.nonBlockingBuffer.Append(
            new EffectGroupBuilder().Add(runtime.UI.ChapterNameBack, new Fade()).Build()
        );
    }

    public override int GetHashCode() => HashCode.Combine(ChapterName);

    public override string ToString() => $"showChapterName: {ChapterName}";
}

public class Say : IScriptBlock
{
    public string name;
    public string content;

    public Say(string name, string content)
    {
        this.name = name;
        this.content = content;
    }

    public override bool Equals(object obj)
    {
        return obj is Say say && name == say.name && content == say.content;
    }

    public override void Execute(AVGRuntime runtime)
    {
        runtime.mainBuffer.Append(
            new EffectGroupBuilder()
                .Add(
                    runtime.UI.TextBox,
                    new LambdaEffect(
                        (node, tween) =>
                        {
                            tween.TweenCallback(
                                Callable.From(() =>
                                {
                                    runtime.UI.CharacterName = name;
                                    runtime.UI.TextBox.Text = content;
                                    runtime.UI.TextBox.VisibleRatio = 0;
                                })
                            );
                            tween.TweenProperty(runtime.UI.TextBox, "visible_ratio", 1.0, 1.0);
                        }
                    )
                )
                .Build()
        );
    }

    public override int GetHashCode() => HashCode.Combine(name, content);

    public override string ToString() => $"Say: name: {name}, content: {content}";
}

public class PlayAudio : IScriptBlock
{
    // path to the audio file("" excluded)
    string path;

    // animation used in audio change
    float FadeInTime;

    public PlayAudio(string path, double FadeInTime = 0.5)
    {
        @continue = true;
        this.path = path;
        this.FadeInTime = (float)FadeInTime;
    }

    public override void Execute(AVGRuntime runtime)
    {
        var audio = GD.Load<AudioStream>(runtime.script.ToResourcePath(path));
        runtime.nonBlockingBuffer.Append(
            new EffectGroupBuilder()
                .Add(
                    runtime.audio,
                    new LambdaEffect(
                        (_, tween) =>
                        {
                            tween.TweenCallback(
                                Callable.From(() =>
                                {
                                    runtime.audio.VolumeDb = -80.0f;
                                    runtime.audio.Play(audio);
                                })
                            );
                            tween
                                .TweenProperty(runtime.audio, "volume_db", 0.0, FadeInTime)
                                .SetTrans(Tween.TransitionType.Expo)
                                .SetEase(Tween.EaseType.Out);
                        }
                    )
                )
                .Build()
        );
    }
}

public class StopAudio : IScriptBlock
{
    float FadeOutTime;

    public StopAudio(double fadeOutTime = 1.0)
    {
        @continue = true;
        this.FadeOutTime = (float)fadeOutTime;
    }

    public override void Execute(AVGRuntime runtime)
    {
        runtime.nonBlockingBuffer.Append(
            new EffectGroupBuilder()
                .Add(
                    runtime.audio,
                    new LambdaEffect(
                        (_, tween) =>
                            tween
                                .TweenProperty(runtime.audio, "volume_db", -80.0, FadeOutTime)
                                .SetTrans(Tween.TransitionType.Expo)
                                .SetEase(Tween.EaseType.In)
                    )
                )
                .Build()
        );
    }
}
