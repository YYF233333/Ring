﻿namespace Ring_Runtime
{
    public class RingScript
    {
        List<IScriptBlock> segments;
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

    public abstract class BuiltInFunction
    {
        /// <summary>
        /// <参数名，参数值>
        /// </summary>
        protected Dictionary<string, string> args;

        public BuiltInFunction() { args = []; ; }
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


    class ShowCharacter : BuiltInFunction, IScriptBlock
    {
        public ShowCharacter(string name, string pos)
        {
            args.Add("name", name);
            args.Add("pos", pos);
        }

        public void Execute(Runtime runtime)
        {
            throw new NotImplementedException();
        }
    }

    class HideCharacter : BuiltInFunction, IScriptBlock
    {
        public HideCharacter(string name)
        {
            args.Add("name", name);
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



    class BG : IScriptBlock
    {
        // path to the new background image
        string path;
        // animation used in show process
        string animation;

        public BG(string path, string animation)
        {
            this.path = path;
            this.animation = animation;
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
