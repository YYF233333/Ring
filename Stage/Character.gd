class_name Character
extends Node2D

## Character is the manage node of a bunch of layers(Sprite2D),
## in sync with psd format.[br]
## Use it like a Sprite2D with static texture, all property change will
## apply to childs automaticly.[br]
## Do not manipulate its child.

## Construct from given layers
func _init(layer_name: Array[String] = [], layer: Array[Texture2D] = []) -> void:
	assert(layer_name.size() == layer.size())
	for i in range(layer_name.size()):
		var name := layer_name[i]
		var texture := layer[i]
		var child := Sprite2D.new()
		child.centered = false
		child.name = name
		child.texture = texture
		add_child(child)
