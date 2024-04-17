extends CanvasLayer

class_name UI

@onready var board = $"../Board" as Board
@onready var center_container = $CenterContainer

func show_game_over():
	center_container.show()


func _on_button_pressed():
	get_parent().cleanup_hook.call()
	#board.process_mode = Node.PROCESS_MODE_INHERIT
	#get_tree().reload_current_scene()
