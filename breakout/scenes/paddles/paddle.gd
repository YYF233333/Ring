extends CharacterBody2D
class_name Paddle

# stat
@export var max_speed : float
@export var acceleration : float
@export var decceleration : float
@export var init_global_pos_y : float

var current_speed : float = 0
var direction

var basic_scale: Vector2

@onready var collision_shape = $CollisionShape2D as CollisionShape2D
@onready var shape = collision_shape.shape as RectangleShape2D
@onready var sprite = $Sprite2D as Sprite2D
@onready var laser_beam = $LaserBeam as LaserBeam

@onready var walls = get_tree().get_first_node_in_group("walls") as Node2D
@onready var left_wall = walls.get_node("Left") as StaticWall
@onready var right_wall = walls.get_node("Right") as StaticWall

@onready var length_change_timer = $LengthChangeTimer as Timer
@onready var speed_change_timer = $SpeedChangeTimer as Timer
@onready var laser_beam_timer = $LaserBeamTimer as Timer

@export var show_floating_text: bool = true
var floating_text_scene = preload("res://breakout/scenes/ui/floating_text.tscn")

@onready var max_scale_x = (right_wall.segment_shape.a.x - left_wall.segment_shape.a.x) / float(shape.size.x)

#debug
#func _set(property: StringName, value: Variant) -> bool:
	#print(property)
	#return false

# buff
var length_change_buff: int = 0:
	set(value):
		length_change_buff = value
		if length_change_buff > 0:
			length_scale = 1.0*length_change_buff+1.0
		elif length_change_buff < 0:
			length_scale = 1.0/(-1.0*length_change_buff+1.0)
		else:
			length_scale = 1.0
		update_stat()
var length_scale: float = 1.0

var speed_change_buff: int = 0:
	set(value):
		speed_change_buff = value
		if speed_change_buff > 0:
			speed_scale = 1.0*speed_change_buff+1.0
		elif speed_change_buff < 0:
			speed_scale = 1.0/(-1.0*speed_change_buff+1.0)
		else:
			speed_scale = 1.0
		update_stat()
var speed_scale: float = 1.0



func _ready():
	$Area2D.body_entered.connect(_on_area_2d_body_entered)
	length_change_timer.timeout.connect(_on_length_change_timer_timeout)
	speed_change_timer.timeout.connect(_on_speed_change_timer_timeout)
	laser_beam_timer.timeout.connect(_on_laser_beam_timer_timeout)
	BreakoutManager.health_changed.connect(_on_health_changed)
	basic_scale = scale
	init_global_pos_y = global_position.y
	
	update_stat()
	
	BreakoutManager.paddle = self


func _physics_process(delta):
	direction = Input.get_action_strength("move_right") - Input.get_action_strength("move_left")
	
	if direction:
		current_speed = move_toward(current_speed, max_speed, acceleration)
		velocity.x = current_speed * direction
	else:
		velocity.x = move_toward(velocity.x, 0, decceleration)
		
	move_and_collide(Vector2(velocity.x, 0) * delta)
	
	global_position.y = init_global_pos_y
	
	
func change_length_buff(layer: int = 1, last_time: float = 5.0):
	"""
	layer: use -1 if shorten
	"""
	length_change_buff += layer
		
	length_change_timer.wait_time = last_time
	length_change_timer.start()
	
	if length_change_buff != 0:
		Utility.wait_with_end_hint(self, last_time)

func change_speed_buff(layer: int = 1, last_time: float = 5.0):
	"""
	layer: use -1 if shorten
	"""
	speed_change_buff += layer
		
	speed_change_timer.wait_time = last_time
	speed_change_timer.start()
	
	if speed_change_buff != 0:
		Utility.wait_with_end_hint(self, last_time)
		
func shoot_laser_beam(last_time: float = 10.0):
	laser_beam.is_casting = true
	
	laser_beam_timer.wait_time = last_time
	laser_beam_timer.start()
	
	
func update_max_scale() -> float:
	max_scale_x = (right_wall.segment_shape.a.x - left_wall.segment_shape.a.x) / float(shape.size.x)
	print(max_scale_x)
	return max_scale_x
	
func update_stat():
	scale.x = min(basic_scale.x * length_scale, max_scale_x)
	# check if exceed walls
	if global_position.x + shape.size.x * scale.x / 2.0 > right_wall.segment_shape.a.x:
		if global_position.x - shape.size.x * scale.x / 2.0 < left_wall.segment_shape.a.x:
			global_position.x = (right_wall.segment_shape.a.x + left_wall.segment_shape.a.x) / 2.0
		else:
			global_position.x = right_wall.segment_shape.a.x - shape.size.x * scale.x / 2.0
	elif global_position.x - shape.size.x * scale.x / 2.0 < left_wall.segment_shape.a.x:
		global_position.x = left_wall.segment_shape.a.x + shape.size.x * scale.x / 2.0
	
	max_speed = ValueManager.paddle_max_speed * speed_scale
	acceleration = ValueManager.paddle_acceleration * speed_scale
	decceleration = ValueManager.paddle_decceleration * speed_scale
	
func _on_length_change_timer_timeout():
	#if length_change_buff > 0:
		#length_change_buff -= 1
	#elif length_change_buff < 0:
		#length_change_buff += 1
	#
	#if length_change_buff != 0:
		#length_change_timer.start()
	length_change_buff = 0
	
func _on_speed_change_timer_timeout():
	#if speed_change_buff > 0:
		#speed_change_buff -= 1
	#elif speed_change_buff < 0:
		#speed_change_buff += 1
	#
	#if speed_change_buff != 0:
		#speed_change_timer.start()
	speed_change_buff = 0
	
func _on_laser_beam_timer_timeout():
	#if speed_change_buff > 0:
		#speed_change_buff -= 1
	#elif speed_change_buff < 0:
		#speed_change_buff += 1
	#
	#if speed_change_buff != 0:
		#speed_change_timer.start()
	laser_beam.is_casting = false
	


func _on_health_changed(value: int):
	if value < 0:
		Utility.flicker_red($%HealthIcon)
		Utility.flicker_red(BreakoutManager.paddle)
		
		if show_floating_text:
			#加载伤害 UI 组件
			var floating_text = floating_text_scene.instantiate() as FloatingText
			get_tree().get_first_node_in_group("foregrounds").add_child(floating_text)
			
			floating_text.global_position = self.global_position + \
			Vector2(randi_range(-8,8),randi_range(-4,4))

			floating_text.start(str(abs(value)), Color(1.0, 0.0, 0.0))
	elif value > 0:
		if show_floating_text:
			#加载伤害 UI 组件
			var floating_text = floating_text_scene.instantiate() as FloatingText
			get_tree().get_first_node_in_group("foregrounds").add_child(floating_text)
			
			floating_text.global_position = self.global_position + \
			Vector2(randi_range(-8,8),randi_range(-4,4))
			
			floating_text.start(str(abs(value)), Color(0.0, 1.0, 0.0))
		
func _on_area_2d_body_entered(body):
	if body is Ball:
		if BreakoutManager.cursed:
			BreakoutManager.take_damage(1)
