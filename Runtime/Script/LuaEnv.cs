using RingEngine.Runtime.Effect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RingEngine.Runtime.Script
{
    struct Params
    {
        public Type type;
        public object defaultValue;

        public Params(Type type, object defaultValue)
        {
            this.type = type;
            this.defaultValue = defaultValue;
        }
    }

    public class LuaEnvironment
    {
        public static List<(string, Delegate)> GetAllEffects()
        {
            var effects = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterface(nameof(IEffect)) != null && t.Namespace == "RingEngine.Runtime.Effect");

            List<(string, Delegate)> ret = [];
            foreach (var effect in effects)
            {
                if (effect.GetConstructors().Length == 0)
                {
                    continue;
                }
                var constructor = effect.GetConstructors()[0];
                var @params = constructor.GetParameters()
                    .Select(param => new Params(param.ParameterType, param.DefaultValue))
                    .ToArray();
                ret.Add((effect.Name, (params object[] args) =>
                {
                    Trace.Assert(@params.Length >= args.Length);
                    List<object> real_args = [];
                    for (int i = 0; i < @params.Length; i++)
                    {
                        if (i < args.Length)
                        {
                            // 使用传入参数，检查类型
                            Trace.Assert(@params[i].type == args[i].GetType());
                            real_args.Add(args[i]);
                        }
                        else
                        {
                            // 使用默认参数，检查是否有默认参数
                            Trace.Assert(DBNull.Value != @params[i].defaultValue);
                            real_args.Add(@params[i].defaultValue);
                        }
                    }
                    return constructor.Invoke(real_args.ToArray());
                }
                ));
            }
            return ret;

        }
    }
}
