namespace RingEngine.Runtime.Script;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

#nullable enable
using ParseResult = (string, IScriptBlock?);

public static class Parser
{
    public static (List<IScriptBlock>, Dictionary<string, int>) Parse(string source)
    {
        List<IScriptBlock> blocks = [];
        Dictionary<string, int> labels = [];
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
            throw new Exception($"Parser Error at \"{source[..Math.Min(source.Length, 20)]}\"");
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
            return new ParseResult(source[match.Length..], new CodeBlock(ident, code));
        }
        return (source, null);
    }

    public static ParseResult ParseShowChapterName(string source)
    {
        var pattern = @"\A#+\s+(?<name>[\s\S]+?)\n";
        var match = Regex.Match(source, pattern);
        if (match.Success)
        {
            return new ParseResult(source[match.Length..], new ShowChapterName(match.Groups["name"].Value.Trim()));
        }
        return new ParseResult(source, null);
    }

    public static ParseResult ParseSay(string source)
    {
        var pattern = @"\A(?<ident>[\s\S]*?)\s*?(:|：)\s*?""(?<content>[\s\S]*?)""";
        var match = Regex.Match(source, pattern);
        if (match.Success)
        {
            return new ParseResult(source[match.Length..], new Say(match.Groups["ident"].Value, match.Groups["content"].Value));
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
    static readonly Dictionary<string, Parser> Parsers = new()
    {
        {"show", ParseShow },
        {"hide", ParseHide },
        {"changeBG", ParseChangeBG },
        {"changeScene", ParseChangeScene },
        {"goto", ParseJumpToLabel },
        {"UIAnim", ParseUIAnim },
        {"stopAudio", ParseStopAudio },
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
        var pattern = @"\Ashow *<img src=""(?<path>[^""]*)""[\s\S]*?/> *as (?<name>\S+) at (`(?<pos>[^`]+)`|(?<pos>\S+))( with (`(?<effect>[^`]+)`|(?<effect>\S+)))?";
        var match = Regex.Match(source, pattern);
        Trace.Assert(match.Success);
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
        Trace.Assert(match.Success);
        var effect_group = match.Groups.GetValueOrDefault("effect");
        var effect = effect_group != null ? effect_group.Value : "";
        return new ParseResult(source[match.Length..], new Hide(match.Groups["name"].Value, effect));
    }

    public static ParseResult ParseChangeBG(string source)
    {
        var pattern = @"\AchangeBG *<img src=""(?<path>[\s\S]*?)""[\s\S]*?/>( *with (`(?<effect>[^`]+)`|(?<effect>\S+)))?";
        var match = Regex.Match(source, pattern);
        Trace.Assert(match.Success);
        var effect_group = match.Groups.GetValueOrDefault("effect");
        var effect = effect_group != null ? effect_group.Value : "";
        return new ParseResult(source[match.Length..], new ChangeBG(match.Groups["path"].Value, effect));
    }

    public static ParseResult ParseChangeScene(string source)
    {
        var pattern = @"\AchangeScene *<img src=""(?<path>[\s\S]*?)""[\s\S]*?/>( *with (`(?<effect>[^`]+)`|(?<effect>\S+)))?";
        var match = Regex.Match(source, pattern);
        Trace.Assert(match.Success);
        var effect_group = match.Groups.GetValueOrDefault("effect");
        var effect = effect_group != null ? effect_group.Value : "";
        return new ParseResult(source[match.Length..], new ChangeScene(match.Groups["path"].Value, effect));
    }

    public static ParseResult ParseJumpToLabel(string source)
    {
        var pattern = @"\Agoto (?<label>[\s\S]+)";
        var match = Regex.Match(source, pattern);
        Trace.Assert(match.Success);
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
        Trace.Assert(match.Success);
        return new ParseResult(source[match.Length..], new UIAnim(match.Groups["effect"].Value));
    }

    public static ParseResult ParseStopAudio(string source)
    {
        var pattern = @"\AstopAudio";
        var match = Regex.Match(source, pattern);
        Trace.Assert(match.Success);
        return new ParseResult(source[match.Length..], new StopAudio());
    }
}
