extends Node
## UI统一交互界面，由于UI结构不确定性，所有输入统一经过该模块派发给剩余部分，
## 所有输出经过该模块派发给UI组件

## 服务器初始化
## param[in]: save_folder 存档文件夹路径
func init() -> void:
	pass

## 服务器析构
func deinit() -> void:
	pass

## 指令派发
func send_message(message: Message) -> void:
	if message is InputMessage:
		if (message as InputMessage).type == InputType.ScriptStepForward:
			Interpreter.step()
