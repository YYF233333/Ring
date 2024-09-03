extends Button

@onready var scene_root = $"../../.."

func _on_pressed() -> void:
	scene_root.Return()
