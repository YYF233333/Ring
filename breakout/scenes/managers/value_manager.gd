extends Node

# player
var player_max_health: int = 10
var player_init_ammo: int = 10
var player_charge_multiplier: float = 1.0

# drop
var laser_beam_damage: float = 2.0 #每秒两次伤害

# paddle
var paddle_max_speed: float = 600
var paddle_acceleration: float = 225
var paddle_decceleration: float = 75

# drop
var current_drop_percent: float = 0.1
var basic_drop_percent: float = 0.1

# ball
var basic_ball_damage: float = 4.0
var ball_damage: int = 4
var ball_init_speed: float = 500.0

var ball_quantity_limit: int = 100
var ball_split_quantity: int = 3

var ball_damage_multiplier: float = 1.0:
	set(value):
		ball_damage_multiplier = value
		_update_ball_damage()

var ball_damage_addition: float = 0.0:
	set(value):
		ball_damage_addition = value
		_update_ball_damage()
		

func reset():
	# 将数值重置为全局基础值
	pass

func _update_ball_damage():
	var _damage = (basic_ball_damage + ball_damage_addition) * ball_damage_multiplier
	ball_damage = ceili(_damage)


func ball_damage_change(multiplier: float = 0.0, addition: float = 0.0):
	ValueManager.ball_damage_multiplier += multiplier
	ValueManager.ball_damage_addition += addition
	
	await get_tree().create_timer(20.0).timeout 
	
	ValueManager.ball_damage_multiplier -= multiplier
	ValueManager.ball_damage_addition -= addition
