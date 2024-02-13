namespace RingEngine.Runtime.Script;
using System.Linq;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using RingEngine.Runtime.Effect;

public class LuaInterpreter
{
    Script interpreter;

    public LuaInterpreter()
    {
        UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
        interpreter = new Script();
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
    }

    public object Eval(string expr)
    {
        return interpreter.DoString($"return {expr}").ToObject();
    }

    public T Eval<T>(string expr)
    {
        return interpreter.DoString($"return {expr}").ToObject<T>();
    }
}
