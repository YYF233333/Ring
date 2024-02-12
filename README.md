# RE: Ring Engine

## TODO List

- [ ] 如何对依赖godot runtime的代码进行单元测试
- [x] 脚本语言选择：lua
- [ ] 跳转语句如何设计？
- [ ] 实现常用的蒙版、Camera、shader动画
- [ ] UI模块
- [ ] 场景序列化实现
- [ ] 存档系统（Using LiteDB）
- [x] <b>Refactor：从UI和Canvas中抽离脚本逻辑到ScriptBlock，动画状态追踪由EffectBuffer负责</b>

动画相关：

- [x] 实现动画缓冲队列

​	   多语句块同时提交时队列按序存放每个语句所用的Effects，首先为语句1分配Tween，监听Tween finish信号，语句1执行完毕（正常/快进）后给语句2分配Tween并执行......

​	   应该可以彻底解决动画相关的并发问题

- [ ] 动画重构，chain effect bug —— group间不存在这个问题（后一组在前一组执行完成后才提交，获取的参数值是最新的），group内Effect默认使用前一个Effect的末值作为初值（不指定初值），单独添加方法来立即修改属性值。

- [ ] 快进的时候chapter name的动画效果如何处理 —— 使用EffectBuffer可以在单语句块中定义多段快进动画

## Tips

- C#脚本抛出异常~~不会传递到Godot侧~~会传过来，但是在调试器里，只有整个.NET运行时崩溃才会在输出里报错

