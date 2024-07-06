extends CanvasLayer

var score = 0

@onready var score_label = $MarginContainer/PanelContainer/MarginContainer/VBoxContainer/FinalScore as Label

# Called when the node enters the scene tree for the first time.
func _ready():
	get_tree().paused = true
	var mini_game_node = get_parent()
	var score = BreakoutManager.score
	score_label.text = "score: " + str(score)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_restart_button_pressed():
	get_tree().paused = false
	get_tree().change_scene_to_file("res://breakout/scenes/breakout/breakout.tscn")


func _on_quit_button_pressed():
	#TODO:把这里改成发出EndGame信号让上层接收
	get_parent().get_parent().EndGame()
