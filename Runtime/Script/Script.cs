using Godot;
using System;
using System.Collections.Generic;

namespace RingEngine.Runtime.Script
{
    public class RingScript
    {
        public List<IScriptBlock> segments;
        public RingScript(string source)
        {
            segments = Parser.Parse(source);
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
        public string position;
        public string effect;


        public Show(string path, string pos, string effect, string name)
        {
            imgName = name;
            imgPath = path;
            position = pos;
            this.effect = effect;
        }

        public override bool Equals(object obj)
        {
            return obj is Show show &&
                   imgName == show.imgName &&
                   imgPath == show.imgPath &&
                   position == show.position &&
                   effect == show.effect;
        }

        public void Execute(Runtime runtime)
        {
            var img = GD.Load("res://" + imgPath);
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(imgName, imgPath, position, effect);
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
