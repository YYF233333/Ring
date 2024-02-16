namespace Test.Runtime.Script;
using NLua;
using RingEngine.Runtime.Effect;

[TestClass]
public class LuaInterpreter
{
    [TestMethod]
    public void TestNlua()
    {
        using var lua = new Lua();
        lua.State.Encoding = System.Text.Encoding.UTF8;
        lua.LoadCLRPackage();
        lua.UseTraceback = true;
        lua.DoString(@"import ('RingEngine', 'RingEngine.Runtime.Effect')");
        Assert.AreEqual(new Dissolve(2.0), lua.DoString("return Dissolve(2)")[0]);
    }

    [TestMethod]
    public void TestMultiReturn()
    {
        using var lua = new Lua();
        lua.State.Encoding = System.Text.Encoding.UTF8;
        lua.LoadCLRPackage();
        lua.UseTraceback = true;
        lua.DoString(@"import ('RingEngine', 'RingEngine.Runtime.Effect')");
        foreach (var ret in lua.DoString("return {1, 2, 3}"))
        {
            Console.WriteLine(ret);
        }
    }
}
