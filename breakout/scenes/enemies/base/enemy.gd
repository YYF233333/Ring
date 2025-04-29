extends CharacterBody2D
class_name Enemy

@export_group("Info")
@export var enemy_id: int
@export var enemy_name: String
@export_multiline var enemy_description: String

@export_group("Basic")
@export var max_health: int
@export var init_health: int
@export var basic_speed: float
@export var basic_acceleration: float
@export var basic_damage: float = 1.0

@export var init_type: int
@export var sprites : Array[Texture]

@export var unstoppable: bool = false #TODO: 能否被击退。现在还不好用，没knockback和板贴贴会卡球

@export_group("Bonus")
@export var point: int
@export var drop_percent_scale: float = 1.0 #1.0表示正常掉率
@export var hit_by_ball_player_charge: int = 1 #被球击中时玩家获得多少充能

@export var lucky_drop: bool = false #会进行lucky drop
@export var unique_drop_name: String

@export_group("", "")

#component
@onready var health_component = $HealthComponent as HealthComponent
@onready var interval_hurt_component = $IntervalHurtComponent as IntervalHurtComponent
@onready var chain_area = $ChainArea as Area2D

@onready var hit_flash_animation_player = $HitFlashAnimationPlayer as AnimationPlayer

@onready var health_bar = $HealthBar as ProgressBar
@onready var health_label = $HealthLabel as Label

@onready var knock_back_timer = $KnockBackTimer as Timer
@onready var attack_cooldown_timer = $AttackCooldownTimer as Timer

@export_group("Basic")
#stat
var type: int
@export var direction: Vector2 = Vector2(1.0, 0.0):
	set(value):
		direction = value
		if direction.is_zero_approx():
			direction = Vector2.ZERO
		else:
			direction = direction.normalized()
#direction表示该enemy的固定移动直线方向
#例如，在无agent(一般写在update_stat)的情况下，默认值Vector2(1.0, 0.0)表示只能沿水平运动
#若为零向量，表示由velocity操纵移动
var current_speed: float:
	set(value):
		current_speed = max(value, 0.0)
var current_direction: Vector2:
	set(value):
		current_direction = value
		if not direction.is_zero_approx():
			current_direction = current_direction.project(direction)
			if current_direction.is_zero_approx():
				current_direction = direction
			else:
				current_direction = current_direction.normalized()
#direction表示该enemy的移动方向

var damage: int

func _ready():
	$Area2D.body_entered.connect(_on_area_2d_body_entered)
	attack_cooldown_timer.timeout.connect(_on_attack_cooldown_timer_timeout)
	health_component.health_change.connect(_on_health_change)
	health_component.died.connect(_on_died)
	add_to_group("enemies")
	
	health_bar.min_value = 0
	health_bar.max_value = max_health
	
	type = init_type
	check_init_type()
	set_type(type)
	show_health()
	
	init_velocity()
	
	_show_lucky_drop()
	
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
			print_debug("what's this?")


func bounce_off_walls(wall : KinematicCollision2D):
	if direction.is_zero_approx():
		velocity = velocity.bounce(wall.get_normal())
	else:
		direction = direction.bounce(wall.get_normal())
		current_direction = current_direction.bounce(wall.get_normal())
	
func bounce_off_bricks(brick : KinematicCollision2D, collider: Brick):
	if direction.is_zero_approx():
		velocity = velocity.bounce(brick.get_normal())
	else:
		direction = direction.bounce(brick.get_normal())
		current_direction = current_direction.bounce(brick.get_normal())
	
func bounce_off_enemies(enemy : KinematicCollision2D, collider: Enemy):
	if direction.is_zero_approx():
		velocity = velocity.bounce(enemy.get_normal())
	else:
		direction = direction.bounce(enemy.get_normal())
		current_direction = current_direction.bounce(enemy.get_normal())
	
func bounce_off_balls(ball : KinematicCollision2D, collider: Ball):
	knocked_back(ball, collider)
	#pass
	
func bounce_off_paddles(paddle : KinematicCollision2D, collider: Paddle):
	knocked_back(paddle, collider)
	
func knocked_back(collision : KinematicCollision2D, collider: Node2D, force: float = 100):
	if unstoppable:
		return
	
	var knock_back_time = 0.5
	
	var knock_back_direction = -(collider.global_position - global_position).normalized()
	var new_velocity = velocity.bounce(collision.get_normal()) + knock_back_direction * force
	
	#if not direction.is_zero_approx():
	if not new_velocity.is_zero_approx():
		current_direction = new_velocity.normalized()
	current_speed = new_velocity.length()
	
	var tween = get_tree().create_tween()
	tween.tween_property(self, "current_speed", 0.0, knock_back_time)
	#else:
		#if not new_velocity.is_zero_approx():
			#current_direction = new_velocity.normalized()
		#current_speed = new_velocity.length()
		#
		#var tween = get_tree().create_tween()
		#tween.tween_property(self, "velocity", Vector2(0,0), knock_back_time)
	
	knock_back_timer.wait_time = knock_back_time
	knock_back_timer.start()

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
	if !knock_back_timer.time_left:
		current_direction = direction
		current_speed = move_toward(current_speed, basic_speed, basic_acceleration * delta)
	velocity = current_direction * current_speed
	#else:
		#velocity = current_direction * current_speed
	#TODO: debug

	
func init_velocity():
	current_speed = basic_speed
	current_direction = direction
	
	velocity = direction * basic_speed
	
	
func check_init_type():
	#debug
	# check if init_type correspond with other stats
	if 0:
		print_debug("type seems not correct")
	

func set_type(new_type : int):
	type = new_type
	if type < 0:
		return
	$Sprite2D.texture = sprites[type]
	
func show_health():
	health_bar.max_value = max_health
	health_bar.value = health_component.current_health
	
	health_label.text = str(health_component.current_health)
	
func _show_lucky_drop():
	if lucky_drop:
		#TODO: 闪金光，表示必定掉落
		$LuckyDropHintParticles.emitting = true
	else:
		$LuckyDropHintParticles.emitting = false
		
func _individual_ready():
	pass


func hit_by_ball(ball: Ball):
	BreakoutManager.ball_hit.emit(hit_by_ball_player_charge)
	physic_hurt(ValueManager.ball_damage)


func hit_by_laser_beam(laser_beam: LaserBeam):
	#TODO: 减速&视效
	current_speed /= 2.0
	physic_hurt(ValueManager.laser_beam_damage)


func physic_hurt(value: int):
	health_component.take_damage(value)
	hit_flash_animation_player.play("hit_flash")
	

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
	
func _on_died():
	BreakoutManager.enemy_beaten.emit(self.name)
	queue_free()
