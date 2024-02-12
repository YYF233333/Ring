using Godot;
using RingEngine.Runtime.Effect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace RingEngine.Runtime.Script
{
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
        public override int GetHashCode()
        {
            return HashCode.Combine(identifier, code);
        }

        public override string ToString()
        {
            return $"CodeBlock: identifier: {identifier}, code: {code}";
        }
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
            runtime.canvas.AddTexture(imgName, texture, Placements.Get(placement));
            if (effect != "")
            {
                runtime.canvas.ApplyEffect(imgName, runtime.interpreter.Eval<IEffect>(effect));
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(imgName, imgPath, placement, effect);
        }

        public override string ToString()
        {
            return $"show: name: {imgName}, path: {imgPath}, placement: {placement}, effect: {effect}";
        }
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
            runtime.canvas.ApplyEffect(name, new Chain([runtime.interpreter.Eval<IEffect>(effect), new Delete()]));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, effect);
        }

        public override string ToString()
        {
            return $"hide: name: {name}, effect: {effect}";
        }
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
                IEffect instance = runtime.interpreter.Eval<IEffect>(effect);
                canvas.ApplyEffect("BG", instance);
                canvas.ApplyEffect(oldBG, new Chain([
                    new Delay(instance.GetDuration()),
                    new LambdaEffect((Node node, Tween tween) =>
                    {
                        tween.TweenCallback(Callable.From(() =>
                        {
                            node.GetParent().RemoveChild(node);
                            node.QueueFree();
                        }));
                        return tween;
                    })
                    ])
                );
            }
            else
            {
                canvas.RemoveChild(oldBG);
                oldBG.QueueFree();
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(imgPath, effect);
        }

        public override string ToString()
        {
            return $"changeBG: path: {imgPath}, effect: {effect}";
        }
    }

    public class ShowChapterName : IScriptBlock
    {
        public string chapterName;

        public ShowChapterName(string chapterName)
        {
            this.chapterName = chapterName;
        }

        public override bool Equals(object obj)
        {
            return obj is ShowChapterName name &&
                   chapterName == name.chapterName;
        }

        public override void Execute(Runtime runtime)
        {
            runtime.UI.ShowChapterName(chapterName);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(chapterName);
        }

        public override string ToString()
        {
            return $"showChapterName: {chapterName}";
        }
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
            runtime.UI.characterName = name;
            runtime.UI.Print(content);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, content);
        }

        public override string ToString()
        {
            return $"Say: name: {name}, content: {content}";
        }
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
}
