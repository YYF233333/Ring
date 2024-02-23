# RE: Ring Engine

## TODO List

- [ ] 如何对依赖godot runtime的代码进行单元测试
- [x] 脚本语言选择：lua
- [ ] 跳转语句如何设计？
    - [ ] jump to file start（无难度）（叫什么名字，goto已经给label jump了）
    - [x] jump to given label
        - [x] 设置label
        - [x] label作用域：文件

    - [ ] 选择支语句设计

- [ ] 实现常用的蒙版、Camera、shader动画
- [ ] UI模块
- [x] <b>Refactor：从UI和Canvas中抽离脚本逻辑到ScriptBlock，动画状态追踪由EffectBuffer负责</b>

脚本相关：

- [x] 添加专门的.lua初始化配置文件

- [x] 将Placement配置迁移到lua侧动态调整

动画相关：

- [x] 实现动画缓冲队列

​	   多语句块同时提交时队列按序存放每个语句所用的Effects，首先为语句1分配Tween，监听Tween finish信号，语句1执行完毕（正常/快进）后给语句2分配Tween并执行......

​	   应该可以彻底解决动画相关的并发问题

动画重构，chain effect bug —— group间不存在这个问题（后一组在前一组执行完成后才提交，获取的参数值是最新的），group内Effect默认使用前一个Effect的末值作为初值（不指定初值），单独添加方法来立即修改属性值。

- [x] 快进的时候chapter name的动画效果如何处理 —— 使用EffectBuffer可以在单语句块中定义多段快进动画，另外添加nonBlockingBuffer处理和其它效果并行的效果
- [ ] 实现多节点效果（Transition）
    - [ ] 基于shader的蒙版动画（还需要添加reverse参数和调整缓动）
    - [ ] 需要的接口暴露给lua运行时


素材格式约定：

给定默认图像分辨率：（待确定）

垂直位置确定：对每个角色添加水平基线，显示时所有基线y值相同

- [ ] 基线值如何给出：
    - 与角色名绑定，用一个json记录每个人的baseline offset，根据立绘索引名称确定是谁的立绘

- [ ] 假设所有角色图像以该分辨率给出且水平居中，根据分辨率计算各个位置的scale和offset



- [ ] 存档系统：
    - [x] 组件序列化/反序列化：全局变量，Canvas，UI，维持Script和LuaInterpreter无状态
    - [x] 存档目录结构
    - [x] 内存暂存快照（History）
    
    能做到的操作：单步回退、任意步回退、指定位置保存、重载后恢复历史
    
    Canvas.scn体积大的原因是素材在挂上去的时候做了resize，这一步重新生成了一个新的Image不对应磁盘上的任何文件，所以保存的时候需要存储，这个问题的原因是当前素材分辨率不一致，需要载入时resize，统一分辨率即可。

## Tips

- C#脚本抛出异常~~不会传递到Godot侧~~会传过来，但是在调试器里，只有整个.NET运行时崩溃才会在输出里报错

- Nlua import AssemblyName里不能有空格
