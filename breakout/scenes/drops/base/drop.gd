extends Node2D
class_name Drop

@export var drop_info: Drop_Info
@export var speed = 300

var picked: bool = false:
	set(value):
		picked = value
		visible = !picked

func _ready():
	$Area2D.body_entered.connect(_on_area_2d_body_entered)
	
	add_to_group("drops")

func _physics_process(delta):
	position.y = position.y + delta * speed


func apply_effect():
	pass


func _on_area_2d_body_entered(body):
	if body is Paddle && !picked:
		picked = true
		await apply_effect()
		queue_free()
