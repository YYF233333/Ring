using RingEngine.Runtime.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Runtime.Script
{
    [TestClass]
    public class LuaEnv
    {
        [TestMethod]
        public void TestGetAllEffects()
        {
            var effects = LuaEnvironment.GetAllEffects();
            foreach (var effect in effects)
            {
                Console.WriteLine(effect.Item1);
            }
        }
    }
}
