extends Node
## UI统一交互界面，由于UI结构不确定性，所有输入统一经过该模块派发给运行时，
## 所有输出经过该模块派发给UI组件


func script_step_forward() -> void:
	Interpreter.step()
