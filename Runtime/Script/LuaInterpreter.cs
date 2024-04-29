namespace RingEngine.Runtime.Script;

using System;
using NLua;

public class LuaInterpreter : IDisposable
{
    Lua interpreter;

    /// <param name="init">
    /// Qol，构建完成后立即执行的初始化代码
    /// 等效于：
    /// <code>
    /// LuaInterpreter interpreter = new LuaInterpreter();
    /// interpreter.Eval(init);
    /// </code>
    /// </param>
    public LuaInterpreter(Runtime runtime, string init = "")
    {
        interpreter = new Lua();
        interpreter.State.Encoding = System.Text.Encoding.UTF8;
        interpreter.LoadCLRPackage();
        interpreter["runtime"] = runtime;
        interpreter["globalvar"] = runtime.global;
        interpreter.DoString(init, "init");
    }

    /// <summary>
    /// 运行表达式<c>expr</c>并返回结果（如果有）
    /// </summary>
    /// <param name="expr">待求值表达式</param>
    /// <returns>表达式求值结果，如果表达式无值返回null</returns>
    public dynamic Eval(string expr)
    {
        var ret = interpreter.DoString($"return {expr}");
        return ret.Length > 0 ? ret[0] : null;
    }

    public void Dispose()
    {
        interpreter.Dispose();
        GC.SuppressFinalize(this);
    }
}
