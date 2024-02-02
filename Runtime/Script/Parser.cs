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
                source = source.Trim();
                var ret = ParseSay(source);
                if (ret.Item2 != null)
                {
                    blocks.Add(ret.Item2);
                    source = ret.Item1;
                    continue;
                }
                ret = ParseCodeBlock(source);
                if (ret.Item2 != null)
                {
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
                throw new Exception($"Parser Error at \"{source.Substring(0, 20)}\"");
            }
            return blocks;
        }

        static (string, string?) ParseIdentifier(string source)
        {
            string pattern = @"\A(?<ident>[\S]*)\s";
            var match = Regex.Match(source, pattern);
            if (match.Success)
            {
                return (source[match.Length..], match.Groups["ident"].Value);
            }
            return (source, null);
        }

        static ParseResult ParseCodeBlock(string source)
        {
            if (!source.StartsWith("```"))
            {
                return new ParseResult(source, null);
            }
            source = source["```".Length..];
            var ret = source.Split("\n", 2);
            var ident = ret[0].Trim();
            ret = ret[1].Split("```", 2);
            var code = ret[0];
            return new ParseResult(ret[1], new CodeBlock(ident, code));
        }

        static ParseResult ParseShowChapterName(string source)
        {
            string pattern = @"[#]+[\s]+(?<name>[\s\S]+)\n";
            var match = Regex.Match(source, pattern);
            if (match.Success)
            {
                return new ParseResult(source[match.Length..], new ShowChapterName(match.Groups["name"].Value));
            }
            return (source, null);
        }

        static ParseResult ParseSay(string source)
        {
            string pattern = @"\A(?<ident>[\S]*)[\s]*:[\s]*""(?<content>[\s\S]*)""";
            var match = Regex.Match(source, pattern);
            if (match.Success)
            {
                return new ParseResult(source[match.Length..], new Say(match.Groups["ident"].Value, match.Groups["content"].Value));
            }
            return (source, null);
        }

        static ParseResult ParseAudio(string source)
        {
            string pattern = @"\A<audio src=""(?<path>[\s\S]*)""></audio>";
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
            {"Show", ParseShow },
            {"Hide", ParseHide },
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
            string pattern = @"\Ashow <img src=""(?<path>[\s\S]*)""[\s\S]*/> at (?<pos>[\S]*)";
            var match = Regex.Match(source, pattern);
            Trace.Assert(match.Success);
            return new ParseResult(source[match.Length..], new Show(match.Groups["path"].Value, match.Groups["pos"].Value));
        }

        public static ParseResult ParseHide(string source)
        {
            string pattern = @"\Ahide(?<name> [\S] *)\n";
            var match = Regex.Match(source, pattern);
            Trace.Assert(match.Success);
            return new ParseResult(source[match.Length..], new Hide(match.Groups["name"].Value));
        }
    }
}
