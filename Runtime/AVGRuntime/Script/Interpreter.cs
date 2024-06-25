namespace RingEngine.Runtime.AVGRuntime.Script;

using System.IO;
using System.Linq;
using Godot;
using Python.Runtime;

public class PythonInterpreter
{
    PyModule scope;

    /// <summary>
    /// 创建一个Python解释器实例
    /// </summary>
    /// <param name="runtime">Ring Runtime</param>
    /// <param name="init">
    /// 初始化脚本
    /// 等效于：
    /// <code>
    /// PythonInterpreter interpreter = new PythonInterpreter();
    /// interpreter.Exec(init);
    /// </code>
    /// </param>
    public PythonInterpreter(AVGRuntime runtime, string projectRoot, string init = "")
    {
        if (!PythonEngine.IsInitialized)
        {
            var PythonDir = new DirectoryInfo(Path.Combine(projectRoot, "python"));
            var Dll = PythonDir
                .GetFiles()
                .Where(file => file.Extension == ".dll")
                .Where(file => file.Name.StartsWith("python3"))
                .Where(file => file.Name != "python3.dll")
                .ToArray();
            if (Dll.Length != 1)
            {
                throw new FileNotFoundException("Cannot identify Python Dll!");
            }
            Runtime.PythonDLL = Dll[0].FullName;
            PythonEngine.Initialize();
        }
        using (Py.GIL())
        {
            scope = Py.CreateScope();
            scope.Set("runtime", runtime);
            scope.Exec(init);
        }
    }

    /// <summary>
    /// 获取Python全局变量
    /// </summary>
    /// <param name="name">变量名</param>
    /// <returns></returns>
    public dynamic this[string name] => scope.Get(name);

    /// <summary>
    /// 对传入的Python表达式求值并返回。由于PyObject自动类型转换的限制，返回值需要赋值给显式注明类型的变量。
    /// </summary>
    public dynamic Eval(string expr)
    {
        using (Py.GIL())
        {
            return scope.Eval(expr);
        }
    }

    /// <summary>
    /// 对传入的Python表达式求值并返回。由于提供了泛型参数，返回值可以直接使用。
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    public T Eval<T>(string expr)
    {
        using (Py.GIL())
        {
            return scope.Eval<T>(expr);
        }
    }

    /// <summary>
    /// 执行一个Python代码段，不返回值。
    /// </summary>
    public void Exec(string code)
    {
        using (Py.GIL())
        {
            scope.Exec(code);
        }
    }
}
