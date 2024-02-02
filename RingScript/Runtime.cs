using MoonSharp.Interpreter;

namespace Ring_Runtime
{

    /// <summary>
    /// 引擎运行时环境
    /// </summary>
    public class Runtime
    {
        // 脚本内嵌代码解释器
        public Script codeInterpreter;
        // 脚本源代码
        public RingScript script;
        public UI UI;
        public Canvas canvas;
        // 持久化数据存储（存档、全局变量）
        public DataBase db;
        // 素材管理
        public Assets assets;


        public Runtime(string Code)
        {
            codeInterpreter = new Script();
            script = new RingScript(Code);
            UI = new UI();
            canvas = new Canvas();
            db = new DataBase();
            assets = new Assets();
        }

        /// <summary>
        /// 运行脚本至下一个中断点
        /// </summary>
        public void Step()
        {

        }

    }

    public class UI
    {
        public UI()
        {
        }
    }

    public class Canvas
    {
        public Canvas() { }
    }

    public class DataBase
    {
        public DataBase() { }
    }

    public class Assets
    {
        public Assets() { }
    }
}
