class_name Stage
extends Node2D
## 舞台对象，全屏幕输出代理，所有显示操作实际执行者

@export var background: Background
@export var characters: Dictionary

func serialize() -> String:
	return ""

func deserialize(data: String) -> void:
	pass
