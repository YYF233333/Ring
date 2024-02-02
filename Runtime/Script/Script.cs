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

    class CodeBlock : IScriptBlock
    {
        // language specified in markdown codeblock(unused)
        string identifier;
        string code;
        public CodeBlock(string identifier, string code)
        {
            this.code = code;
            this.identifier = identifier;
        }

        public void Execute(Runtime runtime)
        {
            runtime.codeInterpreter.DoString(code);
        }
    }


    class Show : IScriptBlock
    {
        string imgPath;
        string position;


        public Show(string imgPath, string pos)
        {
            this.imgPath = imgPath;
            position = pos;
        }

        public void Execute(Runtime runtime)
        {
            var img = GD.Load("res://" + imgPath);
            throw new NotImplementedException();
        }
    }

    class Hide : IScriptBlock
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

    class ShowChapterName : IScriptBlock
    {
        string chapterName;

        public ShowChapterName(string chapterName)
        {
            this.chapterName = chapterName;
        }

        public void Execute(Runtime runtime)
        {
            throw new NotImplementedException();
        }
    }

    class Say : IScriptBlock
    {
        string characterName;
        string content;

        public Say(string characterName, string content)
        {
            this.characterName = characterName;
            this.content = content;
        }

        public void Execute(Runtime runtime)
        {
            throw new NotImplementedException();
        }
    }

    class Audio : IScriptBlock
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
