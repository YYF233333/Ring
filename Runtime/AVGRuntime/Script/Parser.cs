namespace RingEngine.Runtime.AVGRuntime.Script;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Godot;
#nullable enable
using ParseResult = (string, IScriptBlock?);

public class ParserException : Exception
{
    const int MaxLength = 50;

    public ParserException(string source)
        : base(source[..Math.Min(source.Length, MaxLength)]) { }

    public ParserException(string source, int line)
        : base($"Syntax Error at line {line}: \"{source[..Math.Min(source.Length, MaxLength)]}\"")
    { }

    public ParserException(Exception inner, int line)
        : base($"Syntax Error at line {line}: \"{inner.Message}\"") { }
}

public static class Parser
{
    public static (List<IScriptBlock>, Dictionary<string, int>) Parse(string source)
    {
        List<IScriptBlock> blocks = [];
        Dictionary<string, int> labels = [];
        var totalLineNum = source.Count("\n");
        while (source != "")
        {
            source = source.TrimStart();
            if (source == "")
            {
                break;
            }
            try
            {
                var ret = ParseShowChapterName(source);
                if (ret.Item2 != null)
                {
                    blocks.Add(ret.Item2);
                    source = ret.Item1;
                    continue;
                }
                ret = ParsePlayAudio(source);
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
                var label = ParseLabel(source);
                if (label != null)
                {
                    labels[label] = blocks.Count;
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
                    blocks.Add(ret.Item2);
                    source = ret.Item1;
                    continue;
                }
            }
            catch (ParserException e)
            {
                throw new ParserException(e, totalLineNum - source.Count("\n") + 1);
            }
            throw new ParserException(source, totalLineNum - source.Count("\n") + 1);
        }
        return (blocks, labels);
    }

    static (string, string?) ParseIdentifier(string source)
    {
        var pattern = @"\A(?<ident>\S*)\s";
        var match = Regex.Match(source, pattern);
        if (match.Success)
        {
            return (source[match.Length..], match.Groups["ident"].Value);
        }
        return (source, null);
    }

    public static ParseResult ParseCodeBlock(string source)
    {
        var pattern = @"\A```(?<ident>[\s\S]*?)\n(?<code>[\s\S]*?)```";
        var match = Regex.Match(source, pattern);
        if (match.Success)
        {
            var ident = match.Groups["ident"].Value.Trim();
            var code = match.Groups["code"].Value.Trim();
            var ret = new CodeBlock(ident, code);
            // identifier可以用来设置continue属性
            if (ident.StripEdges() is "false" or "False")
            {
                ret.@continue = false;
            }
            return new ParseResult(source[match.Length..], ret);
        }
        return (source, null);
    }

    public static ParseResult ParseShowChapterName(string source)
    {
        var pattern = @"\A#+\s+(?<name>[\s\S]+?)\n";
        var match = Regex.Match(source, pattern);
        if (match.Success)
        {
            return new ParseResult(
                source[match.Length..],
                new ShowChapterName(match.Groups["name"].Value.Trim())
            );
        }
        return new ParseResult(source, null);
    }

    public static ParseResult ParseSay(string source)
    {
        var pattern = @"\A(?<ident>[\s\S]*?)\s*?(:|：)\s*?""(?<content>[\s\S]*?)""";
        var match = Regex.Match(source, pattern);
        if (match.Success)
        {
            return new ParseResult(
                source[match.Length..],
                new Say(match.Groups["ident"].Value, match.Groups["content"].Value)
            );
        }
        return new ParseResult(source, null);
    }

    public static ParseResult ParsePlayAudio(string source)
    {
        var pattern = @"\A<audio src=""(?<path>[\s\S]*?)""></audio>";
        var match = Regex.Match(source, pattern);
        if (match.Success)
        {
            var ret = new PlayAudio(match.Groups["path"].Value);
            return new ParseResult(source[match.Length..], ret);
        }
        return new ParseResult(source, null);
    }

    public static string? ParseLabel(string source)
    {
        var pattern = @"\A\*\*(?<label>[\s\S]+?)\*\*";
        var match = Regex.Match(source, pattern);
        if (match.Success)
        {
            return match.Groups["label"].Value;
        }
        return null;
    }
}

public static class BuiltInFunctionParser
{
    delegate ParseResult Parser(string source);
    static readonly Dictionary<string, Parser> Parsers =
        new()
        {
            { "show", ParseShow },
            { "hide", ParseHide },
            { "changeBG", ParseChangeBG },
            { "changeScene", ParseChangeScene },
            { "goto", ParseJumpToLabel },
            { "UIAnim", ParseUIAnim },
            { "stopAudio", ParseStopAudio },
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
        var pattern =
            @"\Ashow *<img src=""(?<path>[^""]*)""[\s\S]*?/> *as (?<name>\S+) at (`(?<pos>[^`]+)`|(?<pos>\S+))( with (`(?<effect>[^`]+)`|(?<effect>\S+)))?";
        var match = Regex.Match(source, pattern);
        if (!match.Success)
        {
            throw new ParserException(source);
        }
        var effect_group = match.Groups.GetValueOrDefault("effect");
        var effect = effect_group != null ? effect_group.Value : "";
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
        var pattern = @"\Ahide (?<name>[\S]+)( with (?<effect>\S+))?";
        var match = Regex.Match(source, pattern);
        if (!match.Success)
        {
            throw new ParserException(source);
        }
        var effect_group = match.Groups.GetValueOrDefault("effect");
        var effect = effect_group != null ? effect_group.Value : "";
        return new ParseResult(
            source[match.Length..],
            new Hide(match.Groups["name"].Value, effect)
        );
    }

    public static ParseResult ParseChangeBG(string source)
    {
        var pattern =
            @"\AchangeBG *<img src=""(?<path>[\s\S]*?)""[\s\S]*?/>( *with (`(?<effect>[^`]+)`|(?<effect>\S+)))?";
        var match = Regex.Match(source, pattern);
        if (!match.Success)
        {
            throw new ParserException(source);
        }
        var effect_group = match.Groups.GetValueOrDefault("effect");
        var effect = effect_group != null ? effect_group.Value : "";
        return new ParseResult(
            source[match.Length..],
            new ChangeBG(match.Groups["path"].Value, effect)
        );
    }

    public static ParseResult ParseChangeScene(string source)
    {
        var pattern =
            @"\AchangeScene *<img src=""(?<path>[\s\S]*?)""[\s\S]*?/>( *with (`(?<effect>[^`]+)`|(?<effect>\S+)))?";
        var match = Regex.Match(source, pattern);
        if (!match.Success)
        {
            throw new ParserException(source);
        }
        var effect_group = match.Groups.GetValueOrDefault("effect");
        var effect = effect_group != null ? effect_group.Value : "";
        return new ParseResult(
            source[match.Length..],
            new ChangeScene(match.Groups["path"].Value, effect)
        );
    }

    public static ParseResult ParseJumpToLabel(string source)
    {
        var pattern = @"\Agoto (?<label>[\s\S]+)";
        var match = Regex.Match(source, pattern);
        if (!match.Success)
        {
            throw new ParserException(source);
        }
        var label = match.Groups["label"].Value;
        if (label.StartsWith('`'))
        {
            return new ParseResult(source[match.Length..], new JumpToLabel(false, label[1..^1]));
        }
        return new ParseResult(source[match.Length..], new JumpToLabel(true, label));
    }

    public static ParseResult ParseUIAnim(string source)
    {
        var pattern = @"\AUIAnim (`(?<effect>[^`]+)`|(?<effect>\S+))";
        var match = Regex.Match(source, pattern);
        if (!match.Success)
        {
            throw new ParserException(source);
        }
        return new ParseResult(source[match.Length..], new UIAnim(match.Groups["effect"].Value));
    }

    public static ParseResult ParseStopAudio(string source)
    {
        var pattern = @"\AstopAudio";
        var match = Regex.Match(source, pattern);
        if (!match.Success)
        {
            throw new ParserException(source);
        }
        return new ParseResult(source[match.Length..], new StopAudio());
    }
}
