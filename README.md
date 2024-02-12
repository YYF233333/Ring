# RE: Ring Engine

## TODO List

- [ ] 如何对依赖godot runtime的代码进行单元测试
- [ ] 动画重构，chain effect bug
- [x] 脚本语言选择：lua
- [ ] 跳转语句如何设计？
- [ ] 实现常用的蒙版、Camera、shader动画
- [ ] UI模块
- [ ] 场景序列化实现
- [ ] 存档系统（Using LiteDB）
- [ ] 解决动画中断后节点延迟清理问题
- [ ] 快进的时候chapter name的动画效果如何处理

## Tips

- C#脚本抛出异常~~不会传递到Godot侧~~会传过来，但是在调试器里，只有整个.NET运行时崩溃才会在输出里报错

