extends Control

var runtime: Node

## 用于存储选择的全局变量名
var global_var_name: String

var texts: Array[String]

func _ready() -> void:
	for text in texts:
		var tab = ChooseTab.new()
		tab.text = text
		tab.add_theme_font_size_override("", 40)
		$MarginContainer/HBoxContainer.add_child(tab)

func choose(index: int, text := ""):
	runtime.global.SetGlobal(global_var_name, index)
	#TODO: 选项卡消失动画
	queue_free()
