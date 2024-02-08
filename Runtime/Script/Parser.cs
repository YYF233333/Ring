using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

#nullable enable
using ParseResult = (string, RingEngine.Runtime.Script.IScriptBlock?);

namespace RingEngine.Runtime.Script
{
    public static class Parser
    {
        public static List<IScriptBlock> Parse(string source)
        {
            List<IScriptBlock> blocks = [];
            while (source != "")
            {
                source = source.TrimStart();
                if (source == "")
                {
                    break;
                }
                var ret = ParseShowChapterName(source);
                if (ret.Item2 != null)
                {
                    Console.WriteLine("ShowChapterName");
                    blocks.Add(ret.Item2);
                    source = ret.Item1;
                    continue;
                }
                ret = ParseCodeBlock(source);
                if (ret.Item2 != null)
                {
                    Console.WriteLine("CodeBlock");
                    blocks.Add(ret.Item2);
                    source = ret.Item1;
                    continue;
                }
                ret = BuiltInFunctionParser.Parse(source);
                if (ret.Item2 != null)
                {
                    blocks.Add(ret.Item2);
                    source = ret.Item1;
                    continue;
                }
                ret = ParseSay(source);
                if (ret.Item2 != null)
                {
                    Console.WriteLine("Say");
                    blocks.Add(ret.Item2);
                    source = ret.Item1;
                    continue;
                }
                GD.Print(source);
                throw new Exception($"Parser Error at \"{source.Substring(0, 20)}\"");
            }
            return blocks;
        }

        static (string, string?) ParseIdentifier(string source)
        {
            string pattern = @"\A(?<ident>\S*)\s";
            var match = Regex.Match(source, pattern);
            if (match.Success)
            {
                return (source[match.Length..], match.Groups["ident"].Value);
            }
            return (source, null);
        }

        public static ParseResult ParseCodeBlock(string source)
        {
            string pattern = @"\A```(?<ident>[\s\S]*?)\n(?<code>[\s\S]*?)```";
            var match = Regex.Match(source, pattern);
            if (match.Success)
            {
                var ident = match.Groups["ident"].Value.Trim();
                var code = match.Groups["code"].Value.Trim();
                return new ParseResult(source[match.Length..], new CodeBlock(ident, code));
            }
            return (source, null);
        }

        public static ParseResult ParseShowChapterName(string source)
        {
            string pattern = @"\A#+\s+(?<name>[\s\S]+?)\n";
            var match = Regex.Match(source, pattern);
            if (match.Success)
            {
                return new ParseResult(source[match.Length..], new ShowChapterName(match.Groups["name"].Value.Trim()));
            }
            return new ParseResult(source, null);
        }

        public static ParseResult ParseSay(string source)
        {
            string pattern = @"\A(?<ident>\S+)\s*?(:|：)\s*?""(?<content>[\s\S]*?)""";
            var match = Regex.Match(source, pattern);
            if (match.Success)
            {
                return new ParseResult(source[match.Length..], new Say(match.Groups["ident"].Value, match.Groups["content"].Value));
            }
            return new ParseResult(source, null);
        }

        static ParseResult ParseAudio(string source)
        {
            string pattern = @"\A<audio src=""(?<path>[\s\S]*?)""></audio>";
            var match = Regex.Match(source, pattern);
            if (match.Success)
            {
                var ret = new Audio(match.Groups["path"].Value, "");
                return new ParseResult(source[match.Length..], ret);
            }
            return new ParseResult(source, null);
        }

    }

    public static class BuiltInFunctionParser
    {
        delegate ParseResult Parser(string source);
        static readonly Dictionary<string, Parser> Parsers = new Dictionary<string, Parser>
        {
            {"show", ParseShow },
            {"hide", ParseHide },
            {"changeBG", ParseChangeBG }
        };

        public static ParseResult Parse(string source)
        {
            foreach (var funcname in Parsers.Keys)
            {
                if (source.StartsWith(funcname))
                {
                    return Parsers[funcname](source);
                }
            }
            return new ParseResult(source, null);
        }

        public static ParseResult ParseShow(string source)
        {
            string pattern = @"\Ashow <img src=""(?<path>[^""]*)""[\s\S]*?/> as (?<name>\S+) at (?<pos>\S+)( with (?<effect>[\S]+))?";
            var match = Regex.Match(source, pattern);
            Trace.Assert(match.Success);
            var effect_group = match.Groups.GetValueOrDefault("effect");
            string effect = effect_group != null ? effect_group.Value : "";
            return new ParseResult(
                source[match.Length..],
                new Show(
                    match.Groups["path"].Value,
                    match.Groups["pos"].Value,
                    effect,
                    match.Groups["name"].Value
                )
            );
        }

        public static ParseResult ParseHide(string source)
        {
            string pattern = @"\Ahide (?<name>[\S]+)( with (?<effect>\S+))?";
            var match = Regex.Match(source, pattern);
            Trace.Assert(match.Success);
            var effect_group = match.Groups.GetValueOrDefault("effect");
            string effect = effect_group != null ? effect_group.Value : "";
            return new ParseResult(source[match.Length..], new Hide(match.Groups["name"].Value, effect));
        }

        public static ParseResult ParseChangeBG(string source)
        {
            string pattern = @"\AchangeBG to <img src=""(?<path>[\s\S]*?)""[\s\S]*?/>( with (?<effect>\S+))?";
            var match = Regex.Match(source, pattern);
            Trace.Assert(match.Success);
            var effect_group = match.Groups.GetValueOrDefault("effect");
            string effect = effect_group != null ? effect_group.Value : "";
            return new ParseResult(source[match.Length..], new ChangeBG(match.Groups["path"].Value, effect));
        }
    }
}
