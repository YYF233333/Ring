extends Node2D

var ring_sc

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var sc = AssetServer.load_script("res://scriptdemo.md")
	ring_sc = sc.unwrap()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
