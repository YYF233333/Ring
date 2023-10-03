class_name Background
extends Node2D

@export var _sprite: Sprite2D

func _ready() -> void:
	_sprite = Sprite2D.new()
	_sprite.centered = false
	add_child(_sprite)

func show_picture(texture: Texture2D) -> void:
	_sprite.texture = texture
	# 暂时默认拉伸至屏幕大小
	var scale := get_viewport_rect().size / texture.get_size()
	_sprite.scale = scale
