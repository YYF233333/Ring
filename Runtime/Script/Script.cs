namespace RingEngine.Runtime.Script;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Godot;
using RingEngine.Runtime.Effect;

public class RingScript
{
    // 脚本所在文件夹路径
    public string folderPath;
    // 脚本文件名
    public string scriptName;
    public List<IScriptBlock> segments;
    public RingScript(string filePath)
    {
        Trace.Assert(filePath.StartsWith("res://"));
        folderPath = StringExtensions.GetBaseDir(filePath);
        scriptName = Path.GetFileNameWithoutExtension(filePath);
        segments = Parser.Parse(Godot.FileAccess.GetFileAsString(filePath));
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
    public abstract void Execute(Runtime runtime);
}

public class CodeBlock : IScriptBlock
{
    // language specified in markdown codeblock(unused)
    string identifier;
    string code;
    public CodeBlock(string identifier, string code)
    {
        this.code = code;
        this.identifier = identifier;
    }

    // 测试代码使用
    public override bool Equals(object obj)
    {
        return obj is CodeBlock block &&
               identifier == block.identifier &&
               code == block.code;
    }

    public override void Execute(Runtime runtime)
    {
        runtime.interpreter.Eval(code);
    }

    // 测试代码使用
    public override int GetHashCode() => HashCode.Combine(identifier, code);

    public override string ToString() => $"CodeBlock: identifier: {identifier}, code: {code}";
}

public class Show : IScriptBlock
{
    public string imgName;
    public string imgPath;
    public string placement;
    public string effect;


    public Show(string path, string placement, string effect, string name)
    {
        imgName = name;
        imgPath = path;
        this.placement = placement;
        this.effect = effect;
    }

    public override bool Equals(object obj)
    {
        return obj is Show show &&
               imgName == show.imgName &&
               imgPath == show.imgPath &&
               placement == show.placement &&
               effect == show.effect;
    }

    public override void Execute(Runtime runtime)
    {
        var texture = GD.Load<Texture2D>(Path.Combine(runtime.script.folderPath, imgPath));
        var windowSize = new Vector2I(950, 2184);
        var image = texture.GetImage();
        var imageSize = image.GetSize();
        var scale = Math.Min(windowSize.X / (float)imageSize.X, windowSize.Y / (float)imageSize.Y);
        //scale = Math.Max(scale, 1);
        image.Resize((int)(imageSize.X * scale), (int)(imageSize.Y * scale));
        texture = ImageTexture.CreateFromImage(image);
        runtime.canvas.AddTexture(imgName, texture, runtime.interpreter.Eval<Placement>(placement));
        if (effect != "")
        {
            runtime.mainBuffer.Append(new EffectGroupBuilder()
                .Add(runtime.canvas[imgName],
                     runtime.interpreter.Eval<IEffect>(effect))
                .Build());
        }
    }

    public override int GetHashCode() => HashCode.Combine(imgName, imgPath, placement, effect);

    public override string ToString() => $"show: name: {imgName}, path: {imgPath}, placement: {placement}, effect: {effect}";
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
        return obj is Hide hide &&
               name == hide.name &&
               effect == hide.effect;
    }

    public override void Execute(Runtime runtime)
    {
        runtime.mainBuffer.Append(new EffectGroupBuilder()
            .Add(runtime.canvas[name],
                 new Chain(runtime.interpreter.Eval<IEffect>(effect), new Delete()))
            .Build());
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
        return obj is ChangeBG bG &&
               imgPath == bG.imgPath &&
               effect == bG.effect;
    }

    public override void Execute(Runtime runtime)
    {
        var canvas = runtime.canvas;
        var texture = canvas.Stretch(GD.Load<Texture2D>(Path.Combine(runtime.script.folderPath, imgPath)));
        var oldBG = canvas.ReplaceBG(texture);
        if (effect != "")
        {
            var instance = runtime.interpreter.Eval<IEffect>(effect);
            // 对快进来说这里是checkpoint，正常运行时该group和后面的一起提交会连续运行
            runtime.mainBuffer.Append(new EffectGroupBuilder()
                .Add(canvas["BG"], new Chain(new SetAlpha(0), instance))
                .Add(oldBG, new Chain(
                    new Delay(instance.GetDuration()),
                    new LambdaEffect(() =>
                    {
                        canvas.RemoveChild(oldBG);
                        oldBG.QueueFree();
                    })
                ))
                .Build());
        }
        else
        {
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

    public override void Execute(Runtime runtime)
    {
        var canvas = runtime.canvas;
        var texture = canvas.Stretch(GD.Load<Texture2D>(Path.Combine(runtime.script.folderPath, bgPath)));
        if (effect != "")
        {
            var instance = runtime.interpreter.Eval<ITransition>(effect);
            // 对快进来说这里是checkpoint，正常运行时该group和后面的一起提交会连续运行
            runtime.mainBuffer.Append(instance.Build(canvas, texture));
        }
        else
        {
            canvas.AddTexture("BG", texture, Placement.BG, -1);
        }
    }
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
        return obj is ShowChapterName name &&
               ChapterName == name.ChapterName;
    }

    public override void Execute(Runtime runtime)
    {
        var init = new LambdaEffect(() =>
        {
            runtime.UI.ChapterName = ChapterName;
            var ChapterNameBack = runtime.UI.ChapterNameBack;
            ChapterNameBack.Modulate = new Color(ChapterNameBack.Modulate, 0);
        });
        runtime.nonBlockingBuffer.Append(new EffectGroupBuilder().Add(runtime.UI.ChapterNameBack, new Chain(init, new Dissolve())).Build());
        runtime.nonBlockingBuffer.Append(new EffectGroupBuilder().Add(runtime.UI.ChapterNameBack, new Delay(2.0)).Build());
        runtime.nonBlockingBuffer.Append(new EffectGroupBuilder().Add(runtime.UI.ChapterNameBack, new Fade()).Build());
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
        return obj is Say say &&
               name == say.name &&
               content == say.content;
    }

    public override void Execute(Runtime runtime)
    {
        runtime.mainBuffer.Append(new EffectGroupBuilder().Add(runtime.UI.TextBox, new LambdaEffect((Node node, Tween tween) =>
        {
            tween.TweenCallback(Callable.From(() =>
            {
                runtime.UI.CharacterName = name;
                runtime.UI.TextBox.Text = content;
                runtime.UI.TextBox.VisibleRatio = 0;
            }));
            tween.TweenProperty(runtime.UI.TextBox, "visible_ratio", 1.0, 1.0);
        })).Build());
    }

    public override int GetHashCode() => HashCode.Combine(name, content);

    public override string ToString() => $"Say: name: {name}, content: {content}";
}

public class Audio : IScriptBlock
{
    // path to the audio file("" excluded)
    string path;
    // animation used in audio change
    string animation;

    public Audio(string path, string animation)
    {
        this.path = path;
        this.animation = animation;
    }

    public override void Execute(Runtime runtime)
    {
        throw new NotImplementedException();
    }

    public void Print()
    {
        throw new NotImplementedException();
    }
}
