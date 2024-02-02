extends Node
## UI统一交互界面，由于UI结构不确定性，所有输入统一经过该模块派发给运行时，
## 所有输出经过该模块派发给UI组件

## TODO
## 实现输入输出接口
## 输入接口由UI组给出定义和函数原型，运行时组负责实现
## 输出接口反之

func script_step_forward() -> void:
	#Interpreter.step()
	pass

#func _process(delta: float) -> void:
#	if Input.is_action_just_pressed("ui_accept"):
#		Interpreter.step()
