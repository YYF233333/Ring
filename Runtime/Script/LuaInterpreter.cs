namespace RingEngine.Runtime.Script;

using System;
using NLua;
using RingEngine.Runtime.Effect;
using RingEngine.Runtime.Storage;

public class LuaInterpreter : IDisposable
{
    Lua interpreter;
    DataBase global;

    /// <param name="init">
    /// Qol，构建完成后立即执行的初始化代码
    /// 等效于：
    /// <code>
    /// LuaInterpreter interpreter = new LuaInterpreter();
    /// interpreter.Eval(init);
    /// </code>
    /// </param>
    public LuaInterpreter(ref DataBase global, string init = "")
    {
        this.global = global;
        interpreter = new Lua();
        interpreter.State.Encoding = System.Text.Encoding.UTF8;
        interpreter.LoadCLRPackage();
        interpreter["global"] = global;
        interpreter.DoString(init);
        foreach (var (name, effect) in Effects.effects)
        {
            interpreter[name] = effect;
        }
    }

    /// <summary>
    /// 运行表达式<c>expr</c>并返回结果（如果有）
    /// </summary>
    /// <param name="expr"></param>
    /// <returns></returns>
    public object Eval(string expr) => interpreter.DoString($"return {expr}")[0];

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
