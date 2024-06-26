extends StaticBody2D
class_name Brick

@export var brick_id: int
@export var brick_name: String
@export_multiline var brick_description: String

@export var max_health: int
@export var init_health: int

@export var init_type: int
@export var sprites : Array[Texture]

@export var point: int

@export var drop_percent_scale: float = 1.0 #1.0表示正常掉率
@export var bonus: bool = false #会进行lucky drop
@export var hit_by_ball_player_charge: int = 1 #被球击中时玩家获得多少充能

var type: int

@onready var health_component = $HealthComponent as HealthComponent
@onready var interval_hurt_component = $IntervalHurtComponent as IntervalHurtComponent
@onready var chain_area = $ChainArea as Area2D

@onready var health_bar = $HealthBar as ProgressBar
@onready var health_label = $HealthLabel as Label

func _ready():
	$Area2D.body_entered.connect(_on_area_2d_body_entered)
	health_component.health_change.connect(_on_health_change)
	add_to_group("bricks")
	
	health_bar.min_value = 0
	health_bar.max_value = health_component.max_health
	
	type = init_type
	check_init_type()
	set_type(type)
	show_health()
	
	_show_bonus()
	
	_individual_ready()
	
	
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

func _on_health_change():
	var current_health = health_component.current_health
	if current_health > 0:
		show_health()
		
