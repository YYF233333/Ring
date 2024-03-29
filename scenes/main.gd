extends Node

static var cnt := 0

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
	var runtime: Node2D = load("res://Runtime/Runtime.cs").new("res://snapshot")
	runtime.name = "Runtime"
	add_child(runtime)
	var title = $Title
	remove_child(title)
	title.queue_free()
