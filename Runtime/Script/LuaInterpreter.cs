namespace RingEngine.Runtime.Script;

using System;
using NLua;
using RingEngine.Runtime.Effect;

public class LuaInterpreter : IDisposable
{
    Lua interpreter;

    public LuaInterpreter()
    {
        interpreter = new Lua();
        interpreter.State.Encoding = System.Text.Encoding.UTF8;
        interpreter.LoadCLRPackage();
        interpreter.DoString(@"import ('RingEngine', 'RingEngine.Runtime.Effect')");
        foreach (var (name, effect) in Effects.effects)
        {
            interpreter[name] = effect;
        }
    }

    public object Eval(string expr)
    {
        return interpreter.DoString($"return {expr}")[0];
    }

    public T Eval<T>(string expr)
    {
        return (T)interpreter.DoString($"return {expr}")[0];
    }

    public void Dispose()
    {
        interpreter.Dispose();
        GC.SuppressFinalize(this);
    }
}
