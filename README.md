# RE: Ring Engine

## TODO List

- [ ] 将Runtime固化成场景
- [ ] 场景序列化实现
- [ ] 动画系统设计，实现常用的蒙版、Camera、shader动画
- [ ] UI模块
- [ ] 存档系统（Using LiteDB）
- [ ] 解决动画中断后节点延迟清理问题

## Tips

- C#脚本抛出异常~~不会传递到Godot侧~~会传过来，但是在调试器里，只有整个.NET运行时崩溃才会在输出里报错

