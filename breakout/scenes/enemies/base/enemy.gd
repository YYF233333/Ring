extends CharacterBody2D
class_name Enemy

#enemy_info
@export var enemy_id: int
@export var enemy_name: String
@export_multiline var enemy_description: String

@export var max_health: int
@export var init_health: int

@export var basic_speed: float
@export var basic_acceleration: float
@export var basic_damage: float = 1.0

@export var init_type: int
@export var sprites : Array[Texture]

@export var point: int

@export var drop_percent_scale: float = 1.0 #1.0表示正常掉率
@export var bonus: bool = false #会进行lucky drop
@export var hit_by_ball_player_charge: int = 1 #被球击中时玩家获得多少充能

#component
@onready var health_component = $HealthComponent as HealthComponent
@onready var interval_hurt_component = $IntervalHurtComponent as IntervalHurtComponent
@onready var chain_area = $ChainArea as Area2D

@onready var health_bar = $HealthBar as ProgressBar
@onready var health_label = $HealthLabel as Label

@onready var knock_back_timer = $KnockBackTimer as Timer
@onready var attack_cooldown_timer = $AttackCooldownTimer as Timer

#stat
var type: int
@export var current_speed: float = 1.0:
	set(value):
		current_speed = max(value, 1.0)
@export var direction: Vector2 = Vector2(1.0, 0.0) #在无agent(一般写在update_stat)的情况下，该默认值表示只能沿水平运动

var damage: int

func _ready():
	$Area2D.body_entered.connect(_on_area_2d_body_entered)
	attack_cooldown_timer.timeout.connect(_on_attack_cooldown_timer_timeout)
	health_component.health_change.connect(_on_health_change)
	add_to_group("enemies")
	
	health_bar.min_value = 0
	health_bar.max_value = health_component.max_health
	
	type = init_type
	check_init_type()
	set_type(type)
	show_health()
	
	init_velocity()
	
	_show_bonus()
	
	_individual_ready()
	
	
func _physics_process(delta):
	update_stat(delta)
	
	var collision = move_and_collide(velocity * delta)
	var collider:Node2D = collision.get_collider() if collision else null
	
	if collider:
		if collider.is_in_group("walls"):
			bounce_off_walls(collision)
		elif collider.is_in_group("bricks"):
			bounce_off_bricks(collision, collider)
		elif collider.is_in_group("enemies"):
			bounce_off_enemies(collision, collider)
		elif collider.is_in_group("balls"):
			bounce_off_balls(collision, collider)
		elif collider.is_in_group("paddles"):
			bounce_off_paddles(collision, collider)
		else:
			print("what's this?")


func bounce_off_walls(wall : KinematicCollision2D):
	velocity = velocity.bounce(wall.get_normal())
	
func bounce_off_bricks(brick : KinematicCollision2D, collider: Brick):
	velocity = velocity.bounce(brick.get_normal())
	
func bounce_off_enemies(enemy : KinematicCollision2D, collider: Enemy):
	velocity = velocity.bounce(enemy.get_normal())
	
func bounce_off_balls(ball : KinematicCollision2D, collider: Ball):
	knocked_back(ball, collider)
	#pass
	
func bounce_off_paddles(paddle : KinematicCollision2D, collider: Paddle):
	knocked_back(paddle, collider)
	
func knocked_back(collision : KinematicCollision2D, collider: Node2D, force: float = 100):
	var knock_back_time = 0.5
	
	var knock_back_direction = -(collider.global_position - global_position).normalized()
	velocity = velocity.bounce(collision.get_normal()) + knock_back_direction * force
	print(velocity)
	var tween = get_tree().create_tween()
	tween.tween_property(self, "velocity", Vector2(0,0), knock_back_time)
	
	knock_back_timer.start(knock_back_time)

func get_direction_to_paddle(type: String = "center"):
	var paddle_node = BreakoutManager.paddle
	if paddle_node:
		match type:
			"center":
				return (paddle_node.global_position - global_position).normalized()
			"bottom":
				return (paddle_node.global_position - global_position - Vector2(0, $CollisionShape2D.shape.size.y / 2.0)).normalized()
	return Vector2.UP
	
	
func update_stat(delta):
	current_speed = move_toward(current_speed, basic_speed, basic_acceleration * delta)
	if !knock_back_timer.time_left:
		velocity = direction.normalized() * current_speed
	else:
		velocity = direction.normalized() * velocity
	#TODO: debug
	
func init_velocity():
	velocity = direction * basic_speed
	current_speed = basic_speed
	
	
func check_init_type():
	#debug
	# check if init_type correspond with other stats
	if 0:
		print("type seems not correct")
	

func set_type(new_type : int):
	type = new_type
	if type < 0:
		return
	$Sprite2D.texture = sprites[type]
	
	
func show_health():
	health_bar.value = health_component.current_health
	health_bar.max_value = health_component.max_health
	
	health_label.text = str(health_component.current_health)
	
func _show_bonus():
	if bonus:
		#TODO: 闪金光，表示必定掉落
		$BonusParticles.emitting = true
	else:
		$BonusParticles.emitting = false
		
func _individual_ready():
	pass


func hit_by_ball(ball: Ball):
	BreakoutManager.ball_hit.emit(hit_by_ball_player_charge)
	
	if ball.fire_ball_buff > 0:
		interval_hurt_component.fire_debuff_layer += ball.fire_ball_buff
		
	if ball.lightning_ball_buff > 0:
		lightning_hurt(ValueManager.ball_damage)
		chain_lightning(ball.lightning_ball_buff, ValueManager.ball_damage)
	else:
		physic_hurt(ValueManager.ball_damage)

func hit_by_laser_beam(laser_beam: LaserBeam):
	#TODO: 减速&视效
	current_speed /= 2.0
	physic_hurt(ValueManager.laser_beam_damage)
	pass


func physic_hurt(value: int):
	health_component.take_damage(value)
	
func lightning_hurt(value: int):
	health_component.take_damage(value)
			
			
func chain_lightning(lightning_ball_buff: int, damage: int):
	var red_rate = modulate.r
	modulate.r = 0
	modulate.a = 0.5
	var tween = get_tree().create_tween()
	tween.tween_property(self, "modulate", Color(red_rate, modulate.g, modulate.b), 0.25)
	
	if lightning_ball_buff <= 0 or damage < 2:
		return
	
	await get_tree().create_timer(0.1).timeout
	
	var chain_overlapping_bodies = chain_area.get_overlapping_bodies() #TODO: 文档里让请考虑使用信号？
	for body in chain_overlapping_bodies:
		var o = body.owner
		#if o == self:
			#continue
		if o.is_in_group("bricks") or o.is_in_group("enemies"):
			if o.has_method("chain_lightning"):
				o.lightning_hurt(int(damage/2))
				o.chain_lightning(lightning_ball_buff-1, int(damage/2))


func _on_area_2d_body_entered(body):
	if body is Ball:
		hit_by_ball(body)
	elif body is Paddle:
		if !attack_cooldown_timer.time_left:
			attack_player()

func attack_player():
	damage = int(basic_damage)
	if !BreakoutManager.rebound:
		BreakoutManager.take_damage(damage)
	else:
		health_component.take_damage(damage)
	attack_cooldown_timer.start(attack_cooldown_timer.wait_time)
	Utility.screen_shake(0.5)
	

func _on_attack_cooldown_timer_timeout():
	for body in ($Area2D as Area2D).get_overlapping_bodies():
		if body is Paddle:
			attack_player()
		

func _on_health_change():
	var current_health = health_component.current_health
	if current_health > 0:
		show_health()
	

