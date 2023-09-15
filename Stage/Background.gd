class_name Background
extends Node2D

@export var _sprite: Sprite2D

func _ready() -> void:
	_sprite = Sprite2D.new()
	_sprite.centered = false
	add_child(_sprite)

func show_picture(texture: Asset) -> void:
	assert(texture.is_type(AssetType.IMAGE))
	_sprite.texture = texture.unwrap()
