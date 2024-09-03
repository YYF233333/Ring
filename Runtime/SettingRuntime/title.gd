extends Button

@onready var root = $/root
@onready var main = $/root/Main

func _on_pressed() -> void:
	root.remove_child(main)
	main.queue_free()
	var new_main: Node = load("res://scenes/main.tscn").instantiate()
	new_main.name = "Main"
	root.add_child(new_main)
	
