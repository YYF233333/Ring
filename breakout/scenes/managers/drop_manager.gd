extends Node

#@onready var void_drop_tres = preload("res://breakout/scenes/drops/base/drop.tres")
@onready var void_drop_scene = preload("res://breakout/scenes/drops/base/drop.tscn")

@onready var drop_tres_pool: Array[Resource] = [
	#preload("res://breakout/scenes/drops/fast/fast_drop.tres"),
	#preload("res://breakout/scenes/drops/fire_ball/fire_ball_drop.tres"),
	#preload("res://breakout/scenes/drops/lightning_ball/lightning_ball_drop.tres"),
	#preload("res://breakout/scenes/drops/long/long_drop.tres"),
	#preload("res://breakout/scenes/drops/short/short_drop.tres"),
	#preload("res://breakout/scenes/drops/slow/slow_drop.tres"),
	#preload("res://breakout/scenes/drops/laser_beam/laser_beam_drop.tres"),
	#preload("res://breakout/scenes/drops/split/split_drop.tres")
]

@onready var drop_scene_pool: Array[PackedScene] = [
	#preload("res://breakout/scenes/drops/fast/fast_drop.tscn"),
	#preload("res://breakout/scenes/drops/fire_ball/fire_ball_drop.tscn"),
	#preload("res://breakout/scenes/drops/lightning_ball/lightning_ball_drop.tscn"),
	#preload("res://breakout/scenes/drops/long/long_drop.tscn"),
	#preload("res://breakout/scenes/drops/short/short_drop.tscn"),
	#preload("res://breakout/scenes/drops/slow/slow_drop.tscn"),
	#preload("res://breakout/scenes/drops/laser_beam/laser_beam_drop.tscn"),
	#preload("res://breakout/scenes/drops/split/split_drop.tscn")
]

@onready var lucky_drop_tres_pool: Array[Resource] = [
	#preload("res://breakout/scenes/drops/fire_ball/fire_ball_drop.tres"),
	#preload("res://breakout/scenes/drops/lightning_ball/lightning_ball_drop.tres"),
	#preload("res://breakout/scenes/drops/long/long_drop.tres"),
	#preload("res://breakout/scenes/drops/laser_beam/laser_beam_drop.tres"),
	#preload("res://breakout/scenes/drops/split/split_drop.tres")
]

@onready var lucky_drop_scene_pool: Array[PackedScene] = [
	#preload("res://breakout/scenes/drops/fire_ball/fire_ball_drop.tscn"),
	#preload("res://breakout/scenes/drops/lightning_ball/lightning_ball_drop.tscn"),
	#preload("res://breakout/scenes/drops/long/long_drop.tscn"),
	#preload("res://breakout/scenes/drops/laser_beam/laser_beam_drop.tscn"),
	#preload("res://breakout/scenes/drops/split/split_drop.tscn")
]

# please ensure one-to-one correspondence of drop_tres_pool and drop_scene_pool

var each_drop_percent: Array[float] = []
var each_lucky_drop_percent: Array[float] = []

var id_to_drop_scene_index: Array[int] = []
var name_to_drop_scene_index: Array[String] = []


func _ready():
	_get_drop_percent()
	_get_lucky_drop_percent()
	_get_id_to_drop_scene_index()


func _get_drop_percent():
	each_drop_percent.clear()
	var percent_sum = 0.0
	for drop_tres in (drop_tres_pool as Array[Drop_Info]):
		var current_drop_percent = pow(2.0, -drop_tres.drop_percent_level)
		each_drop_percent.append(current_drop_percent)
		percent_sum += current_drop_percent
	each_drop_percent.assign(each_drop_percent.map(func(num):
		return num / percent_sum
	))
	
func _get_lucky_drop_percent():
	each_lucky_drop_percent.clear()
	var percent_sum = 0.0
	for lucky_drop_tres in (lucky_drop_tres_pool as Array[Drop_Info]):
		var current_lucky_drop_percent = pow(2.0, -lucky_drop_tres.drop_percent_level)
		each_lucky_drop_percent.append(current_lucky_drop_percent)
		percent_sum += current_lucky_drop_percent
	each_lucky_drop_percent.assign(each_lucky_drop_percent.map(func(num):
		return num / percent_sum
	))
		
func _get_id_to_drop_scene_index():
	id_to_drop_scene_index.clear()
	for drop_tres in (drop_tres_pool as Array[Drop_Info]):
		id_to_drop_scene_index.append(drop_tres.drop_id)
	
func _get_name_to_drop_scene_index():
	name_to_drop_scene_index.clear()
	for drop_tres in (drop_tres_pool as Array[Drop_Info]):
		name_to_drop_scene_index.append(drop_tres.drop_name)

func get_drop_scene_by_id(drop_id: int) -> PackedScene:
	return drop_scene_pool[id_to_drop_scene_index.find(drop_id)]

func get_drop_scene_by_name(drop_name: String) -> PackedScene:
	return drop_scene_pool[name_to_drop_scene_index.find(drop_name)]


func get_random_drop_scene(each_drop_percent: Array[float] = self.each_drop_percent) -> PackedScene:
	var random_number = randf()
	var current_percent = 0
	for i in range(each_drop_percent.size()):
		current_percent += each_drop_percent[i]
		if random_number < current_percent:
			return drop_scene_pool[i]
	return void_drop_scene
	

func spawn_random_drop(global_pos: Vector2, each_drop_percent: Array[float] = self.each_drop_percent):
	var random_drop_scene_instance = get_random_drop_scene(each_drop_percent).instantiate() as Drop
	var drops = get_tree().get_first_node_in_group("drops") as Node2D
	drops.add_child(random_drop_scene_instance)
	random_drop_scene_instance.global_position = global_pos


func get_random_lucky_drop_scene(each_lucky_drop_percent: Array[float] = self.each_lucky_drop_percent) -> PackedScene:
	var random_number = randf()
	var current_percent = 0
	for i in range(each_lucky_drop_percent.size()):
		current_percent += each_lucky_drop_percent[i]
		if random_number < current_percent:
			return lucky_drop_scene_pool[i]
	return void_drop_scene


func spawn_random_lucky_drop(global_pos: Vector2, each_lucky_drop_percent: Array[float] = self.each_lucky_drop_percent):
	var random_lucky_drop_scene_instance = get_random_lucky_drop_scene(each_lucky_drop_percent).instantiate() as Drop
	var drops = get_tree().get_first_node_in_group("drops") as Node2D
	drops.add_child(random_lucky_drop_scene_instance)
	random_lucky_drop_scene_instance.global_position = global_pos
