using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RingEngine.Runtime.Effect;
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
            foreach (var (name, constructor) in effects)
            {
                Console.WriteLine(name);
            }
            var cons = effects["Dissolve"];
            // Invoke不会进行类型转换
            //Assert.AreEqual(new Dissolve(2), effects["Dissolve"].Invoke(2));会报错
            Assert.AreEqual(new Dissolve(2), effects["Dissolve"](2.0));
        }

        [TestMethod]
        public void Test()
        {
            UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
            var interpreter = new MoonSharp.Interpreter.Script();
            foreach (var (name, func) in LuaEnvironment.GetAllEffects())
            {
                interpreter.Globals[name] = DynValue.NewCallback(
                    (_, args) =>
                    {
                        var param = args.GetArray().Select(x => x.ToObject());
                        return DynValue.FromObject(interpreter, func(param.ToArray()));
                    }
                );
            }
            foreach (var (name, effect) in Effects.effects)
            {
                interpreter.Globals[name] = effect;
            }
            Assert.AreEqual(new Dissolve(), interpreter.DoString("return dissolve").ToObject());
        }
    }
}
