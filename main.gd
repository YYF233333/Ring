extends Node2D

var ring_sc

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var sc = AssetServer.load_script("res://scriptdemo.md")
	ring_sc = sc
	Interpreter.load_script(sc)
	var logo := load("res://icon.svg")
	var chara := Character.new(["logo"], [logo])
	#add_child(chara)
	Interpreter.step()
	Interpreter.step()
	Interpreter.step()
	Interpreter.step()
	Interpreter.step()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
