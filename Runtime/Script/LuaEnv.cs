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

    /// <summary>
    /// 静态构造函数，避免lua侧调用new
    /// </summary>
    /// <param name="args">构造函数参数</param>
    /// <returns>生成的对象</returns>
    public delegate object Constructor(params object[] args);

    /// <summary>
    /// 带类型限制的静态构造函数，生成IEffect对象
    /// </summary>
    public delegate IEffect EffectConstructor(params object[] args);

    public class LuaEnvironment
    {
        /// <summary>
        /// 收集RingEngine.Runtime.Effect中所有的<typeparamref name="IEffect"/>类，将构造函数包装成static func传递给MoonSharp
        /// <list type="table">
        /// <listheader><description>限制：</description></listheader>
        /// <item><description>每个类有且仅有一个构造函数（多于一个则随机选取）</description></item>
        /// <item><description>static constructor调用时不会做隐式类型转换</description></item>
        /// </list>
        /// </summary>
        /// <returns>static constructor indexed by class name</returns>
        public static Dictionary<string, EffectConstructor> GetAllEffects()
        {
            var effects = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterface(nameof(IEffect)) != null && t.Namespace == "RingEngine.Runtime.Effect");

            Dictionary<string, EffectConstructor> ret = [];
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
                ret.Add(effect.Name, (params object[] args) =>
                {
                    args ??= [];
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
                    return (IEffect)constructor.Invoke(real_args.ToArray());
                }
                );
            }
            return ret;

        }
    }
}
