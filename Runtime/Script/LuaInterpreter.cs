using MoonSharp.Interpreter.Interop;
using MoonSharp.Interpreter;
using RingEngine.Runtime.Effect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingEngine.Runtime.Script
{
    public class LuaInterpreter
    {
        MoonSharp.Interpreter.Script interpreter;

        public LuaInterpreter()
        {
            UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
            interpreter = new MoonSharp.Interpreter.Script();
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
}
