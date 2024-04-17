extends Node

static var cnt := 0

@onready var minigame_scene: PackedScene = preload("res://Tetromino/minigame.tscn")

func game_start():
	var tween := create_tween()
	tween.tween_property($Black, "modulate:a", 1.0, 1.0)
	tween.tween_interval(0.5)
	tween.tween_callback(func():
		var runtime: Node2D = load("res://Runtime/Runtime.cs").new()
		runtime.name = "Runtime"
		add_child(runtime)
		runtime.process_mode = Node.PROCESS_MODE_DISABLED
		var title = $Title
		remove_child(title)
		title.queue_free()
		)
	tween.tween_property($Black, "modulate:a", 0.0, 1.0)
	tween.tween_callback(func():
		$Runtime.process_mode = Node.PROCESS_MODE_INHERIT
		)

func load_snapshot():
	var runtime: Node2D = load("res://Runtime/Runtime.cs").new()
	runtime.LoadSnapshot("res://snapshot")
	runtime.name = "Runtime"
	add_child(runtime)
	var title = $Title
	remove_child(title)
	title.queue_free()

func load_mini_game():
	var minigame_instance = minigame_scene.instantiate()
	minigame_instance.name = "MiniGame"
	minigame_instance.cleanup_hook = Callable(end_mini_game)
	$Runtime.process_mode = Node.PROCESS_MODE_DISABLED
	$Runtime.visible = false
	add_child(minigame_instance)

func end_mini_game():
	$MiniGame.queue_free()
	$Runtime.process_mode = Node.PROCESS_MODE_INHERIT
	$Runtime.visible = true
