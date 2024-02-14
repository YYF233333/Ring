namespace Test.Runtime.Script;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using NLua;
using RingEngine.Runtime.Effect;
using RingEngine.Runtime.Script;
using System;
using System.Linq;

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
    public void TestNlua()
    {
        using (Lua lua = new Lua())
        {
            lua.State.Encoding = System.Text.Encoding.UTF8;
            lua.LoadCLRPackage();
            lua.UseTraceback = true;
            lua.DoString(@"import ('RingEngine', 'RingEngine.Runtime.Effect')");
            Assert.AreEqual(new Dissolve(2.0), lua.DoString("return Dissolve(2)")[0]);
        }
    }

    [TestMethod]
    public void Test()
    {
        UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
        var interpreter = new Script();
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
