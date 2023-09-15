class_name Message
## Message基类，message目的地由子类类型与消息类型共同决定

var from: Node # Debug use
var args: Array # 消息参数

func _init(_from: Node, _args: Array = []) -> void:
	from = _from
	args = _args

