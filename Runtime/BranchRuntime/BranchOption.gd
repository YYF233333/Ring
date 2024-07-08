class_name BranchOption
extends Button

signal option_choosed(id: int)

var id: int

func _init(id: int, text: String) -> void:
	self.id = id
	self.text = text
	self.custom_minimum_size = Vector2(900, 90)
	self.add_theme_font_size_override("font_size", 40)
	self.pressed.connect(_on_button_pressed)
	

func _on_button_pressed() -> void:
	option_choosed.emit(id)
