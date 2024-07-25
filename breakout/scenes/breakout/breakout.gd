extends Node2D
class_name Breakout

@onready var camera = $Camera2D as Camera2D

var shake: bool = false
var shake_time: int = 0
var shake_force: float = 1.0

# Called when the node enters the scene tree for the first time.
func _ready():
	BreakoutManager.breakout = self


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if shake:
		shake_time += 1
		var target_offset = Vector2(cos(shake_time)*4.0*shake_force, sin(shake_time)*2.25*shake_force)
		camera.offset = lerp(camera.offset, target_offset, 2)
	elif shake_time:
		shake_time = 0
		shake_force = 1.0
		camera.offset = Vector2(0,0)

	#debug
	if Input.is_action_just_pressed("menu"):
		BreakoutManager._on_failed()
