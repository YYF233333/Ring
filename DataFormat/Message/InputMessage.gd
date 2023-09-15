class_name InputMessage
extends Message
## 输入消息，表示UI侧向运行时发送的消息

var type: InputType

func _init(_from: Node, _type: InputType, _args: Array = []) -> void:
	from = _from
	type = _type
	args = _args

static func script_step_forward(from: Node, args: Array = []) -> InputMessage:
	return InputMessage.new(from, InputType.ScriptStepForward, args)
