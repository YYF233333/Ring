extends Node2D
## 舞台对象，全屏幕输出代理，所有显示操作实际执行者

## child node
var background: Background
var characters: Dictionary

## 从序列化数据中加载场景
func _ready() -> void:
	background = Background.new()
	background.name = "Background"
	add_child(background)
	characters = {}

func serialize() -> String:
	return ""

func deserialize(data: String) -> void:
	pass
