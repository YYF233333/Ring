using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RingEngine.Runtime.Script;

namespace Test.Runtime.Script
{
    [TestClass]
    public class TestParser
    {
        const string script = @"# 章节标题

角色名:""台词""

```python
#show_character()
```

changeBG to <img src=""bg1.png"" style=""zoom: 10%;"" /> with dissolve

show <img src=""assets/chara.png"" style=""zoom:25%;"" /> as 红叶 at farleft with dissolve

";

        [TestMethod]
        public void TestParse()
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

            foreach (var pair in answer.Zip(ret))
            {
                Assert.AreEqual(pair.First, pair.Second);
            }
        }
        [TestMethod]
        public void TestParseShowChapterName()
        {
            var ret = Parser.ParseShowChapterName("# Chapter 1\n");
            Assert.AreEqual("", ret.Item1);
            Assert.IsNotNull(ret.Item2);
            Assert.AreEqual(new ShowChapterName("Chapter 1"), ret.Item2);
        }

        [TestMethod]
        public void TestParseSay()
        {
            var ret = Parser.ParseSay("红叶 : \"台词 abab;:.\n\"");
            Assert.AreEqual("", ret.Item1);
            Assert.IsNotNull(ret.Item2);
            Assert.AreEqual(new Say("红叶", "台词 abab;:.\n"), ret.Item2);
        }

        [TestMethod]
        public void TestParseSay2()
        {
            var ret = Parser.ParseSay("红叶 ： \"台词 abab;:.\n\"");
            Assert.AreEqual("", ret.Item1);
            Assert.IsNotNull(ret.Item2);
            Assert.AreEqual(new Say("红叶", "台词 abab;:.\n"), ret.Item2);
        }

        [TestMethod]
        public void TestParseSayNotEnd()
        {
            var ret = Parser.ParseSay("红叶 : \"台词\"\nother content");
            Assert.AreEqual("\nother content", ret.Item1);
            Assert.IsNotNull(ret.Item2);
            Assert.AreEqual(new Say("红叶", "台词"), ret.Item2);
        }

        [TestMethod]
        public void TestParseCodeBlock()
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
        public void TestParseShow()
        {
            var ret = BuiltInFunctionParser.ParseShow(@"show <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> as 红叶 at left");
            Assert.AreEqual("", ret.Item1);
            Assert.IsNotNull(ret.Item2);
            Show block = (Show)ret.Item2;
            Assert.AreEqual(new Show("assets/bg2.jpg", "left", "", "红叶"), block);
        }

        [TestMethod]
        public void TestParseShowWithEffect()
        {
            var ret = BuiltInFunctionParser.ParseShow(@"show <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> as 红叶 at left with dissolve");
            Assert.AreEqual("", ret.Item1);
            Assert.IsNotNull(ret.Item2);
            Show block = (Show)ret.Item2;
            Assert.AreEqual(new Show("assets/bg2.jpg", "left", "dissolve", "红叶"), block);
        }

        [TestMethod]
        public void TestParseChangeBG()
        {
            var ret = BuiltInFunctionParser.ParseChangeBG(@"changeBG to <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" />");
            Assert.AreEqual("", ret.Item1);
            Assert.IsNotNull(ret.Item2);
            ChangeBG block = (ChangeBG)ret.Item2;
            Assert.AreEqual(new ChangeBG("assets/bg2.jpg", ""), block);
        }

        [TestMethod]
        public void TestParseChangeBGWithEffect()
        {
            var ret = BuiltInFunctionParser.ParseChangeBG(@"changeBG to <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> with dissolve");
            Assert.AreEqual("", ret.Item1);
            Assert.IsNotNull(ret.Item2);
            ChangeBG block = (ChangeBG)ret.Item2;
            Assert.AreEqual(new ChangeBG("assets/bg2.jpg", "dissolve"), block);
        }
    }
}
