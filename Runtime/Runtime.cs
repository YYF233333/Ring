namespace RingEngine.Runtime;

using System;
using System.Collections.Generic;
using Godot;
using RingEngine.Runtime.Storage;

/// <summary>
/// 子运行时抽象
/// </summary>
public interface ISubRuntime
{
    /// <summary>
    /// 子运行时名称，唯一识别符
    /// </summary>
    public string RuntimeName { get; }

    /// <summary>
    /// 保存运行时当前状态。可选实现，不支持存档的子运行时可以忽略。
    /// </summary>
    /// <returns>运行时快照</returns>
    public ISnapshot Save()
    {
        if (OS.HasFeature("editor"))
        {
            GD.Print(
                $"SubRuntime {RuntimeName} does not implement Save Method, "
                    + $"you may forget to implement this optional method."
            );
        }
        return null;
    }

    /// <summary>
    /// 从快照中恢复运行时状态。可选实现，不支持存档的子运行时可以忽略。
    /// </summary>
    /// <param name="snapshot">运行时状态快照</param>
    public void LoadSnapshot(ISnapshot snapshot)
    {
        throw new NotImplementedException(
            $"SubRuntime {RuntimeName} does not implement LoadSnapshot Method, "
                + $"you may forget to implement this optional method."
        );
    }

    /// <summary>
    /// 接收由其它子运行时传来的消息。具体的消息格式由子运行时协商。
    /// </summary>
    /// <param name="runtimeName">发送方名称</param>
    /// <param name="message">更新消息</param>
    public void GetMessage(string runtimeName, object message);
}

/// <summary>
/// 运行时快照接口
/// </summary>
public interface ISnapshot
{
    /// <summary>
    /// 将当前运行时状态保存到指定文件夹（文件夹可能不存在）
    /// </summary>
    /// <param name="folder">存档文件夹路径</param>
    public void Save(string folder);

    /// <summary>
    /// 从指定文件夹加载运行时状态
    /// </summary>
    /// <param name="folder">存档文件夹路径</param>
    public void Load(string folder);
}

public enum SwitchMode
{
    /// <summary>
    /// 卸载当前子运行时，恢复时需要加载快照
    /// </summary>
    Unload,

    /// <summary>
    /// 仅暂停处理当前子运行时，不生成快照
    /// </summary>
    Pause,
}

public partial class Runtime : Node
{
    // 进度无关全局设置
    public GlobalConfig config;

    public Dictionary<string, Func<ISubRuntime>> subRuntimes =
        new()
        {
            { "AVGRuntime", () => (AVGRuntime.AVGRuntime)GD.Load<CSharpScript>("res://Runtime/AVGRuntime/AVGRuntime.cs").New() },
            { "Breakout", () => (Root)GD.Load<CSharpScript>("res://breakout/Root.cs").New() }
        };

    public Dictionary<string, ISnapshot> snapshots = [];

    // TODO: 子场景切换动画组


    public Runtime()
    {
        config = new GlobalConfig()
        {
            YBaseTable = new Dictionary<string, double> { { "红叶", 600 } }
        };
        var defaultRuntime = (Node)subRuntimes[config.DefaultRuntime]();
        AddChild(defaultRuntime);
    }

    /// <summary>
    /// 切换子运行时，由当前运行的子运行时调用并指定后继子运行时。
    /// </summary>
    /// <typeparam name="T">子运行时必须为Node且实现ISubRuntime</typeparam>
    /// <param name="self">当前子运行时（即该方法调用方）</param>
    /// <param name="nextSubRuntimeName">要激活的子运行时</param>
    /// <param name="message">要传递的消息</param>
    /// <param name="switchMode">切换模式，见<see cref="SwitchMode"/></param>
    public void SwitchRuntime<T>(
        T self,
        string nextSubRuntimeName,
        object message,
        SwitchMode switchMode = SwitchMode.Unload
    )
        where T : Node, ISubRuntime
    {
        switch (switchMode)
        {
            case SwitchMode.Unload:
                var snapshot = self.Save();
                if (snapshot != null)
                {
                    snapshots[self.RuntimeName] = snapshot;
                }
                RemoveChild(self);
                self.QueueFree();
                break;
            case SwitchMode.Pause:
                self.ProcessMode = ProcessModeEnum.Disabled;
                break;
            default:
                throw new ArgumentException($"Invalid SwitchMode {switchMode}");
        }

        var nextSubRuntime = subRuntimes[nextSubRuntimeName]();
        AddChild(nextSubRuntime as Node);

        if (snapshots.TryGetValue(nextSubRuntimeName, out var value))
        {
            nextSubRuntime.LoadSnapshot(value);
            snapshots.Remove(nextSubRuntimeName);
        }
        if (message != null)
        {
            nextSubRuntime.GetMessage(self.RuntimeName, message);
        }
    }
}
