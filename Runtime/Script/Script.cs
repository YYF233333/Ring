using Godot;
using RingEngine.Runtime.Effect;
using System;
using System.Collections.Generic;
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
            folderPath = ProjectSettings.LocalizePath(Path.GetDirectoryName(filePath));
            scriptName = Path.GetFileNameWithoutExtension(filePath);
            segments = Parser.Parse(File.ReadAllText(filePath));
        }
    }

    public interface IScriptBlock
    {
        /// <summary>
        /// 执行当前代码块
        /// </summary>
        /// <param name="runtime">运行时环境</param>
        public void Execute(Runtime runtime);
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

        public override bool Equals(object obj)
        {
            return obj is CodeBlock block &&
                   identifier == block.identifier &&
                   code == block.code;
        }

        public void Execute(Runtime runtime)
        {
            runtime.codeInterpreter.DoString(code);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(identifier, code);
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

        public void Execute(Runtime runtime)
        {
            var texture = GD.Load<Texture2D>(Path.Combine(runtime.script.folderPath, imgPath));
            runtime.canvas.AddTexture(imgName, texture, Placements.Get(placement));
            runtime.canvas.ApplyEffect(imgName, Effects.Get(effect));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(imgName, imgPath, placement, effect);
        }
    }

    public class Hide : IScriptBlock
    {
        string name;
        public Hide(string name)
        {
            this.name = name;
        }

        public void Execute(Runtime runtime)
        {
            throw new NotImplementedException();
        }
    }

    public class ChangeBG : IScriptBlock
    {
        public string imgPath;
        public string effect;

        public ChangeBG(string path, string effect)
        {
            this.imgPath = path;
            this.effect = effect;
        }

        public override bool Equals(object obj)
        {
            return obj is ChangeBG bG &&
                   imgPath == bG.imgPath &&
                   effect == bG.effect;
        }

        public void Execute(Runtime runtime)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(imgPath, effect);
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

        public void Execute(Runtime runtime)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(chapterName);
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

        public void Execute(Runtime runtime)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, content);
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

        public void Execute(Runtime runtime)
        {
            throw new NotImplementedException();
        }
    }
}
