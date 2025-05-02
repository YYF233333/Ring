extends Node

const debug: bool = false

"""
信号中心
游戏管理中心
player状态处理中心
"""

signal point_scored(point)

signal goal_text_changed(text: String)

signal ammo_lost(value: int)
signal ammo_changed

signal health_changed(value: int)

signal skill_charge(value: int)
signal skill_charged
signal skill_changed
signal try_use_skill

signal ball_lost(ball: Ball)
signal ball_hit(charge: int)

signal enemy_beaten(enemy_name: StringName)
signal brick_broken(brick_name: StringName)

signal failed
signal track_lost #剧情杀，特殊结算
signal goal_clear(level_name: String, goal_name: String) #关卡小目标达成，通常准备转阶段
signal level_clear(level_name: String) #关卡完成

var breakout: Breakout
var skill: Skill
var balls: Balls
var paddle: Paddle
var consumables: Consumables

var is_game_ended : bool = false
var ammo : int:
	set(value):
		ammo = max(value, 0)
		ammo_changed.emit()
var score : int = 0

var current_health: int:
	set(value):
		var old_health = current_health
		current_health = min(max(value, 0), max_health)
		if _health_ready:
			health_changed.emit(current_health-old_health)
		else:
			health_changed.emit(0) #为了防止初始化时血量变化被识别为回血
var max_health: int:
	set(value):
		max_health = value
		current_health = current_health
		
var cursed: bool = false:
	set(value):
		cursed = value
		if value:
			(paddle.sprite as Sprite2D).texture = preload("res://breakout/assets/CursedPaddle.png")
		else:
			(paddle.sprite as Sprite2D).texture = preload("res://breakout/assets/DefaultPaddle.png")
var rebound: bool = false:
	set(value):
		rebound = value
		if value:
			(paddle.sprite as Sprite2D).texture = preload("res://breakout/assets/GoldenPaddle.png")
		else:
			(paddle.sprite as Sprite2D).texture = preload("res://breakout/assets/DefaultPaddle.png")
var blessed: bool = false:
	set(value):
		blessed = value
		#视效
		
var init_message: Dictionary
var _health_ready: bool = false

func _ready():
	try_use_skill.connect(_on_try_use_skill)
	point_scored.connect(_on_point_scored)
	ball_lost.connect(_on_ball_lost)
	ammo_lost.connect(_on_ammo_lost)
	
	failed.connect(_on_failed)
	track_lost.connect(_on_track_lost)
	level_clear.connect(_on_level_clear)
#
	#call_deferred("reset") #在breakout里调用，不然restart会寄


func receive_init_message(message: Dictionary):
	init_message = message.duplicate(true)
	call_deferred("reset")
	
	
func send_message() -> Dictionary:
	var ret_message = Dictionary()
	
	var player_consumables = Dictionary()
	for consumable in consumables.consumable_nodes:
		var consumable_name = (consumable as Consumable).consumable_info.consumable_name
		var rest_times = (consumable as Consumable).rest_times
		var transformed = (consumable as Consumable).transformed
		player_consumables[consumable_name] = Dictionary()
		player_consumables[consumable_name]["rest_times"] = rest_times
		player_consumables[consumable_name]["transformed"] = transformed
	ret_message["player_consumables"] = player_consumables
		
	var level_result = Dictionary()
	ret_message["level_result"] = level_result
		
	return ret_message
	
	
func reset():
	if init_message:
		print_debug(init_message)
		var player_consumables: Dictionary = init_message["player_consumables"]
		consumables.reset()
		for consumable_name in player_consumables.keys():
			var rest_times: int = player_consumables[consumable_name]["rest_times"]
			var transformed: bool = player_consumables[consumable_name]["transformed"]
			var consumable: Consumable = ResourceManager.get_consumable_scene_by_name(consumable_name).instantiate()
			consumable.rest_times = rest_times
			consumable.transformed = transformed
			consumables.add_consumable_instance(consumable)


		var selected_skill: String = init_message["selected_skill"]
		skill = ResourceManager.get_skill_scene_by_name(selected_skill).instantiate() as Skill
		skill_changed.emit()

		var selected_policy: PackedStringArray = init_message["selected_policy"]
		#TODO

		var player_max_health: int = init_message["player_max_health"]
		ValueManager.player_max_health = player_max_health
		
		var player_init_ammo: int = init_message["player_init_ammo"]
		ValueManager.player_init_ammo = player_init_ammo
		
		var current_level: String = init_message["current_level"]
		var current_level_scene = ResourceManager.get_level_scene_by_name(current_level) as PackedScene
		if current_level_scene:
			var previous_level = get_tree().get_first_node_in_group("levels") as Node2D
			previous_level.add_sibling(current_level_scene.instantiate())
			previous_level.queue_free()
		
		var current_level_manager_name: String = init_message["current_level_manager"]
		var current_level_manager = ResourceManager.get_level_manager_by_name(current_level_manager_name) as PackedScene
		if current_level_manager:
			var previous_level_manager = get_tree().get_first_node_in_group("level_managers") as Node2D
			previous_level_manager.add_sibling(current_level_manager.instantiate())
			previous_level_manager.queue_free()
		
	
	if skill:
		skill.current_charge = 0
	
	_health_ready = false
	max_health = ValueManager.player_max_health #小心顺序，要先设置max_health
	current_health = max_health
	_health_ready = true
	
	ammo = ValueManager.player_init_ammo
	score = 0


func get_charge_percent():
	if skill.max_charge <= 0:
		return 1
		
	return skill.current_charge/float(skill.max_charge)


func take_damage(value: int):
	#debug
	current_health -= value
	Callable(check_death).call_deferred()
	
	
func get_health_percent():
	if max_health <= 0:
		return 0
	else:
		return min(current_health/float(max_health), 1)
		
		
func check_death():
	if current_health <= 0:
		failed.emit()


func clear_bricks():
	var bricks = get_tree().get_nodes_in_group("bricks")
	for brick in bricks:
		brick.queue_free()


func clear_enemies():
	var enemies = get_tree().get_nodes_in_group("enemies")
	for enemy in enemies:
		enemy.queue_free()


func pause_game():
	breakout.process_mode = Node.PROCESS_MODE_DISABLED
	
	
func continue_game():
	breakout.process_mode = Node.PROCESS_MODE_INHERIT


func _on_try_use_skill():
	if skill:
		skill.try_use()


func _on_point_scored(point: int):
	score += point


func _on_ball_lost(ball: Ball):
	var balls = get_tree().get_first_node_in_group("balls") as Balls
	balls.del_ball(ball)


func _on_ammo_lost(value: int):
	ammo -= value
	if ammo == 0:
		print_debug("ammo run out") #TODO: hint label & restart bottom
	
	
func _on_failed():
	get_tree().paused = true
	var end_screen = preload("res://breakout/scenes/screens/EndMiniGameScreen.tscn").instantiate()
	breakout.add_child(end_screen)
	
	
func _on_track_lost():
	get_tree().paused = true
	var end_screen = preload("res://breakout/scenes/screens/EndMiniGameScreen.tscn").instantiate()
	breakout.add_child(end_screen)
	
	
func _on_level_clear(level_name: String):
	get_tree().paused = true
	var end_screen = preload("res://breakout/scenes/screens/EndMiniGameScreen.tscn").instantiate()
	breakout.add_child(end_screen)
	
