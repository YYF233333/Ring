namespace Test.Runtime.Script;
using System.Collections.Generic;
using System.Linq;
using RingEngine.Runtime.Script;

[TestClass]
public class TestParser
{
    const string script = @"# 章节标题

角色名:""台词""

```python
#show_character()
```

changeBG <img src=""bg1.png"" style=""zoom: 10%;"" /> with dissolve

show <img src=""assets/chara.png"" style=""zoom:25%;"" /> as 红叶 at farleft with dissolve

";

    [TestMethod]
    public void Parse()
    {
        var ret = Parser.Parse(script);
        Assert.AreEqual(5, ret.Count);
        List<IScriptBlock> answer = [
            new ShowChapterName("章节标题"),
            new Say("角色名", "台词"),
            new CodeBlock("python", "#show_character()"),
            new ChangeBG("bg1.png", "dissolve"),
            new Show("assets/chara.png", "farleft", "dissolve", "红叶")
        ];

        foreach (var (reference, actual) in answer.Zip(ret))
        {
            Assert.AreEqual(reference, actual);
        }
    }

    [TestMethod]
    public void ParseShowChapterName()
    {
        var ret = Parser.ParseShowChapterName("# Chapter 1\n");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Assert.AreEqual(new ShowChapterName("Chapter 1"), ret.Item2);
    }

    [TestMethod]
    public void ParseSay()
    {
        var ret = Parser.ParseSay("红叶 : \"台词 abab;:.\n\"");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Assert.AreEqual(new Say("红叶", "台词 abab;:.\n"), ret.Item2);
    }

    [TestMethod]
    public void ParseSay2()
    {
        var ret = Parser.ParseSay("红叶 ： \"台词 abab;:.\n\"");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Assert.AreEqual(new Say("红叶", "台词 abab;:.\n"), ret.Item2);
    }

    [TestMethod]
    public void ParseSayNotEnd()
    {
        var ret = Parser.ParseSay("红叶 : \"台词\"\nother content");
        Assert.AreEqual("\nother content", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Assert.AreEqual(new Say("红叶", "台词"), ret.Item2);
    }

    [TestMethod]
    public void ParseCodeBlock()
    {
        var ret = Parser.ParseCodeBlock(@"``` Python
# 控制逻辑
# 代码块可以操作运行时提供的舞台对象(StageObject)
# e.g.
#show_character()
```");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Assert.AreEqual(new CodeBlock("Python", @"# 控制逻辑
# 代码块可以操作运行时提供的舞台对象(StageObject)
# e.g.
#show_character()"), ret.Item2);
    }
}

[TestClass]
public class TestBuiltInParser
{
    [TestMethod]
    public void ParseShow()
    {
        var ret = BuiltInFunctionParser.ParseShow(@"show <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> as 红叶 at left");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Show block = (Show)ret.Item2;
        Assert.AreEqual(new Show("assets/bg2.jpg", "left", "", "红叶"), block);
    }

    [TestMethod]
    public void ParseShowWithEffect()
    {
        var ret = BuiltInFunctionParser.ParseShow(@"show <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> as 红叶 at left with dissolve");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Show block = (Show)ret.Item2;
        Assert.AreEqual(new Show("assets/bg2.jpg", "left", "dissolve", "红叶"), block);
    }

    [TestMethod]
    public void ParseShowIndent()
    {
        var ret = BuiltInFunctionParser.ParseShow(@"show<img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" />as 红叶 at left");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Show block = (Show)ret.Item2;
        Assert.AreEqual(new Show("assets/bg2.jpg", "left", "", "红叶"), block);
        ret = BuiltInFunctionParser.ParseShow(@"show   <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" />   as 红叶 at left");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        block = (Show)ret.Item2;
        Assert.AreEqual(new Show("assets/bg2.jpg", "left", "", "红叶"), block);
    }

    [TestMethod]
    public void ParseShowWithInlineCode()
    {
        var ret = BuiltInFunctionParser.ParseShow(@"show <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> as 红叶 at left with `Dissolve(2.0, 0.5)`");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Show block = (Show)ret.Item2;
        Assert.AreEqual(new Show("assets/bg2.jpg", "left", "Dissolve(2.0, 0.5)", "红叶"), block);
    }

    [TestMethod]
    public void ParseChangeBG()
    {
        var ret = BuiltInFunctionParser.ParseChangeBG(@"changeBG <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" />");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        ChangeBG block = (ChangeBG)ret.Item2;
        Assert.AreEqual(new ChangeBG("assets/bg2.jpg", ""), block);
    }

    [TestMethod]
    public void ParseChangeBGWithEffect()
    {
        var ret = BuiltInFunctionParser.ParseChangeBG(@"changeBG <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> with dissolve");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        ChangeBG block = (ChangeBG)ret.Item2;
        Assert.AreEqual(new ChangeBG("assets/bg2.jpg", "dissolve"), block);
    }

    [TestMethod]
    public void ParseChangeBGIndent()
    {
        var ret = BuiltInFunctionParser.ParseChangeBG(@"changeBG<img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" />with dissolve");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        ChangeBG block = (ChangeBG)ret.Item2;
        Assert.AreEqual(new ChangeBG("assets/bg2.jpg", "dissolve"), block);
        ret = BuiltInFunctionParser.ParseChangeBG(@"changeBG   <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" />   with dissolve");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        block = (ChangeBG)ret.Item2;
        Assert.AreEqual(new ChangeBG("assets/bg2.jpg", "dissolve"), block);
    }

    [TestMethod]
    public void ParseChangeBGWithInlineCode()
    {
        var ret = BuiltInFunctionParser.ParseChangeBG(@"changeBG <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> with `Dissolve(2.0, 0.5)`");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        ChangeBG block = (ChangeBG)ret.Item2;
        Assert.AreEqual(new ChangeBG("assets/bg2.jpg", "Dissolve(2.0, 0.5)"), block);
    }

    [TestMethod]
    public void ParseHide()
    {
        var ret = BuiltInFunctionParser.ParseHide(@"hide 红叶");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Hide block = (Hide)ret.Item2;
        Assert.AreEqual(new Hide("红叶", ""), block);
    }

    [TestMethod]
    public void ParseHideWithEffect()
    {
        var ret = BuiltInFunctionParser.ParseHide(@"hide 红叶 with dissolve");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Hide block = (Hide)ret.Item2;
        Assert.AreEqual(new Hide("红叶", "dissolve"), block);
    }
}
