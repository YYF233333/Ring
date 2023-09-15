class_name OutputMessage
extends Message
## 输出消息，运行时侧向UI侧发送的消息

var type: OutputType

func _init(_from: Node, _type: OutputType, _args: Array = []) -> void:
	from = _from
	type = _type
	args = _args
