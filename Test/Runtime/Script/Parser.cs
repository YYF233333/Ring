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
        [TestMethod]
        public void TestMethod1()
        {
        }
    }

    [TestClass]
    public class TestBuiltInParser
    {
        [TestMethod]
        public void TestParseShow()
        {
            var ret = BuiltInFunctionParser.ParseShow(@"show <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> at left");
            Assert.IsNotNull(ret.Item2);
        }
    }
}
