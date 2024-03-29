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
        interpreter.DoString(init, "init");
    }

    /// <summary>
    /// 运行表达式<c>expr</c>并返回结果（如果有）
    /// </summary>
    /// <param name="expr"></param>
    /// <returns></returns>
    public dynamic Eval(string expr) => interpreter.DoString($"return {expr}")[0];

    /// <summary>
    /// 运行表达式<c>expr</c>并返回结果（如果有）
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="expr"></param>
    /// <returns></returns>
    public T Eval<T>(string expr) => (T)interpreter.DoString($"return {expr}")[0];

    public void Dispose()
    {
        interpreter.Dispose();
        GC.SuppressFinalize(this);
    }
}
