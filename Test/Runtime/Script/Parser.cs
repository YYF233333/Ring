namespace Test.Runtime.Script;

using System.Collections.Generic;
using System.Linq;
using RingEngine.Runtime.AVGRuntime.Script;

[TestClass]
public class TestParser
{
    const string script =
        @"# 章节标题

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
        (var ret, _) = Parser.Parse(script);
        Assert.AreEqual(5, ret.Count);
        List<IScriptBlock> answer =
        [
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
    public void Parse2()
    {
        var script = File.ReadAllText("C:\\Users\\Yufeng Ying\\Desktop\\Ring\\main.md");
        (var ret, _) = Parser.Parse(script);
    }

    [TestMethod]
    public void ParseLabel()
    {
        var ret = Parser.ParseLabel(@"**选择支1**");
        Assert.AreEqual("选择支1", ret.Item2);
    }

    [TestMethod]
    public void ParseShowChapterName()
    {
        var ret = Parser.ParseShowChapterName("# Chapter 1\n");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Assert.AreEqual(new ShowChapterName("Chapter 1"), ret.Item2[0]);
    }

    [TestMethod]
    public void ParseSay()
    {
        var ret = Parser.ParseSay("红叶 : \"台词 abab;:.\n\"");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Assert.AreEqual(new Say("红叶", "台词 abab;:.\n"), ret.Item2[0]);
    }

    [TestMethod]
    public void ParseSay2()
    {
        var ret = Parser.ParseSay("红叶 ： \"台词 abab;:.\n\"");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Assert.AreEqual(new Say("红叶", "台词 abab;:.\n"), ret.Item2[0]);
    }

    [TestMethod]
    public void ParseSayEmpty()
    {
        var ret = Parser.ParseSay("： \"台词 abab;:.\n\"");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Assert.AreEqual(new Say("", "台词 abab;:.\n"), ret.Item2[0]);
    }

    [TestMethod]
    public void ParseSayNotEnd()
    {
        var ret = Parser.ParseSay("红叶 : \"台词\"\nother content");
        Assert.AreEqual("\nother content", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Assert.AreEqual(new Say("红叶", "台词"), ret.Item2[0]);
    }

    [TestMethod]
    public void ParseCodeBlock()
    {
        var ret = Parser.ParseCodeBlock(
            @"``` Python
# 控制逻辑
# 代码块可以操作运行时提供的舞台对象(StageObject)
# e.g.
#show_character()
```"
        );
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        Assert.AreEqual(
            new CodeBlock(
                "Python",
                @"# 控制逻辑
# 代码块可以操作运行时提供的舞台对象(StageObject)
# e.g.
#show_character()"
            ),
            ret.Item2[0]
        );
    }

    [TestMethod]
    public void ParseBranch()
    {
        var (remain, blocks) = Parser.ParseBranch(
            @"| 竖排 |
        | ----------------------------- |
        | 选项1                         |
        | 选项2                         |
        | 选项3                         |"
        );

        Assert.AreEqual("", remain);
        Assert.IsNotNull(blocks);
        var branch = (Branch)blocks[0];
        Assert.AreEqual(Branch.BranchType.Vertical, branch.Type);
        foreach (
            var (reference, actual) in new string[] { "选项1", "选项2", "选项3" }.Zip(branch.Options)
        )
        {
            Assert.AreEqual(reference, actual);
        }
    }

    [TestMethod]
    public void ParseBranchwithLabel()
    {
        var (remain, blocks) = Parser.ParseBranch(
            @"| 竖排 |        |
        | ----------------------------- | ------ |
        | 选项1                         | label1 |
        | 选项2                         | label2 |
        | 选项3                         | label3 |"
        );

        Assert.AreEqual("", remain);
        Assert.IsNotNull(blocks);
        var branch = (Branch)blocks[0];
        var gotoCode = (CodeBlock)blocks[1];
        Assert.AreEqual(Branch.BranchType.Vertical, branch.Type);
        foreach (
            var (reference, actual) in new string[] { "选项1", "选项2", "选项3" }.Zip(branch.Options)
        )
        {
            Assert.AreEqual(reference, actual);
        }
        Assert.AreEqual(
            gotoCode.code,
            @"g = runtime.Global
if g.LastChosenOptionId == 0:
    runtime.Goto(""label1"")
if g.LastChosenOptionId == 1:
    runtime.Goto(""label2"")
if g.LastChosenOptionId == 2:
    runtime.Goto(""label3"")
"
        );
    }
}

[TestClass]
public class TestBuiltInParser
{
    [TestMethod]
    public void ParseShow()
    {
        var ret = BuiltInFunctionParser.ParseShow(
            @"show <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> as 红叶 at left"
        );
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        var block = (Show)ret.Item2[0];
        Assert.AreEqual(new Show("assets/bg2.jpg", "left", "", "红叶"), block);
    }

    [TestMethod]
    public void ParseShowWithEffect()
    {
        var ret = BuiltInFunctionParser.ParseShow(
            @"show <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> as 红叶 at left with dissolve"
        );
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        var block = (Show)ret.Item2[0];
        Assert.AreEqual(new Show("assets/bg2.jpg", "left", "dissolve", "红叶"), block);
    }

    [TestMethod]
    public void ParseShowIndent()
    {
        var ret = BuiltInFunctionParser.ParseShow(
            @"show<img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" />as 红叶 at left"
        );
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        var block = (Show)ret.Item2[0];
        Assert.AreEqual(new Show("assets/bg2.jpg", "left", "", "红叶"), block);
        ret = BuiltInFunctionParser.ParseShow(
            @"show   <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" />   as 红叶 at left"
        );
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        block = (Show)ret.Item2[0];
        Assert.AreEqual(new Show("assets/bg2.jpg", "left", "", "红叶"), block);
    }

    [TestMethod]
    public void ParseShowWithInlineCode()
    {
        var ret = BuiltInFunctionParser.ParseShow(
            @"show <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> as 红叶 at left with `Dissolve(2.0, 0.5)`"
        );
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        var block = (Show)ret.Item2[0];
        Assert.AreEqual(new Show("assets/bg2.jpg", "left", "Dissolve(2.0, 0.5)", "红叶"), block);
    }

    [TestMethod]
    public void ParseChangeBG()
    {
        var ret = BuiltInFunctionParser.ParseChangeBG(
            @"changeBG <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" />"
        );
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        var block = (ChangeBG)ret.Item2[0];
        Assert.AreEqual(new ChangeBG("assets/bg2.jpg", ""), block);
    }

    [TestMethod]
    public void ParseChangeBGWithEffect()
    {
        var ret = BuiltInFunctionParser.ParseChangeBG(
            @"changeBG <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> with dissolve"
        );
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        var block = (ChangeBG)ret.Item2[0];
        Assert.AreEqual(new ChangeBG("assets/bg2.jpg", "dissolve"), block);
    }

    [TestMethod]
    public void ParseChangeBGIndent()
    {
        var ret = BuiltInFunctionParser.ParseChangeBG(
            @"changeBG<img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" />with dissolve"
        );
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        var block = (ChangeBG)ret.Item2[0];
        Assert.AreEqual(new ChangeBG("assets/bg2.jpg", "dissolve"), block);
        ret = BuiltInFunctionParser.ParseChangeBG(
            @"changeBG   <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" />   with dissolve"
        );
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        block = (ChangeBG)ret.Item2[0];
        Assert.AreEqual(new ChangeBG("assets/bg2.jpg", "dissolve"), block);
    }

    [TestMethod]
    public void ParseChangeBGWithInlineCode()
    {
        var ret = BuiltInFunctionParser.ParseChangeBG(
            @"changeBG <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> with `Dissolve(2.0, 0.5)`"
        );
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        var block = (ChangeBG)ret.Item2[0];
        Assert.AreEqual(new ChangeBG("assets/bg2.jpg", "Dissolve(2.0, 0.5)"), block);
    }

    [TestMethod]
    public void ParseHide()
    {
        var ret = BuiltInFunctionParser.ParseHide(@"hide 红叶");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        var block = (Hide)ret.Item2[0];
        Assert.AreEqual(new Hide("红叶", ""), block);
    }

    [TestMethod]
    public void ParseHideWithEffect()
    {
        var ret = BuiltInFunctionParser.ParseHide(@"hide 红叶 with dissolve");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        var block = (Hide)ret.Item2[0];
        Assert.AreEqual(new Hide("红叶", "dissolve"), block);
    }

    [TestMethod]
    public void ParseJumpToLabel()
    {
        var ret = BuiltInFunctionParser.ParseJumpToLabel(@"goto label1");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        var block = (JumpToLabel)ret.Item2[0];
        Assert.AreEqual(new JumpToLabel(true, "label1"), block);

        ret = BuiltInFunctionParser.ParseJumpToLabel(@"goto `label expression`");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        block = (JumpToLabel)ret.Item2[0];
        Assert.AreEqual(new JumpToLabel(false, "label expression"), block);
    }

    [TestMethod]
    public void ParseUIAnim()
    {
        var ret = BuiltInFunctionParser.ParseUIAnim(@"UIAnim dissolve");
        Assert.AreEqual("", ret.Item1);
        Assert.IsNotNull(ret.Item2);
        var block = (UIAnim)ret.Item2[0];
        Assert.AreEqual(new UIAnim("dissolve"), block);
    }
}
