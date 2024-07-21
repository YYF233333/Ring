extends Node

@onready var void_drop_tres = preload("res://breakout/scenes/drops/base/drop.tres")
@onready var void_drop_scene = preload("res://breakout/scenes/drops/base/drop.tscn")

@onready var drop_tres_pool: Array[Resource] = [
	preload("res://breakout/scenes/drops/fast/fast_drop.tres"),
	preload("res://breakout/scenes/drops/long/long_drop.tres"),
	preload("res://breakout/scenes/drops/short/short_drop.tres"),
	preload("res://breakout/scenes/drops/slow/slow_drop.tres"),
	preload("res://breakout/scenes/drops/laser_beam/laser_beam_drop.tres"),
	preload("res://breakout/scenes/drops/split/split_drop.tres"),
	preload("res://breakout/scenes/drops/charge/charge_drop.tres"),
	preload("res://breakout/scenes/drops/heal/heal_drop.tres")
]

@onready var drop_scene_pool: Array[PackedScene] = [
	preload("res://breakout/scenes/drops/fast/fast_drop.tscn"),
	preload("res://breakout/scenes/drops/long/long_drop.tscn"),
	preload("res://breakout/scenes/drops/short/short_drop.tscn"),
	preload("res://breakout/scenes/drops/slow/slow_drop.tscn"),
	preload("res://breakout/scenes/drops/laser_beam/laser_beam_drop.tscn"),
	preload("res://breakout/scenes/drops/split/split_drop.tscn"),
	preload("res://breakout/scenes/drops/charge/charge_drop.tscn"),
	preload("res://breakout/scenes/drops/heal/heal_drop.tscn")
]

@onready var lucky_drop_tres_pool: Array[Resource] = [
	preload("res://breakout/scenes/drops/long/long_drop.tres"),
	preload("res://breakout/scenes/drops/laser_beam/laser_beam_drop.tres"),
	preload("res://breakout/scenes/drops/split/split_drop.tres"),
	preload("res://breakout/scenes/drops/charge/charge_drop.tres"),
	preload("res://breakout/scenes/drops/heal/heal_drop.tres")
]

@onready var lucky_drop_scene_pool: Array[PackedScene] = [
	preload("res://breakout/scenes/drops/long/long_drop.tscn"),
	preload("res://breakout/scenes/drops/laser_beam/laser_beam_drop.tscn"),
	preload("res://breakout/scenes/drops/split/split_drop.tscn"),
	preload("res://breakout/scenes/drops/charge/charge_drop.tscn"),
	preload("res://breakout/scenes/drops/heal/heal_drop.tscn")
]

@onready var void_consumable_tres = preload("res://breakout/scenes/consumables/base/consumable.tres")
@onready var void_consumable_scene = preload("res://breakout/scenes/consumables/base/consumable.tscn")

@onready var consumable_tres_pool: Array[Resource] = [
	preload("res://breakout/scenes/consumables/calculator/calculator.tres"), 
	preload("res://breakout/scenes/consumables/communicator/communicator.tres"), 
	preload("res://breakout/scenes/consumables/old_model/old_model.tres"), 
	preload("res://breakout/scenes/consumables/package/package.tres"), 
	preload("res://breakout/scenes/consumables/passport/passport.tres"), 
]

@onready var consumable_scene_pool: Array[PackedScene] = [
	preload("res://breakout/scenes/consumables/calculator/calculator.tscn"), 
	preload("res://breakout/scenes/consumables/communicator/communicator.tscn"), 
	preload("res://breakout/scenes/consumables/old_model/old_model.tscn"), 
	preload("res://breakout/scenes/consumables/package/package.tscn"), 
	preload("res://breakout/scenes/consumables/passport/passport.tscn"), 
]

@onready var void_skill_tres = preload("res://breakout/scenes/skills/base/skill.tres")
@onready var void_skill_scene = preload("res://breakout/scenes/skills/base/skill.tscn")

@onready var skill_tres_pool: Array[Resource] = [
	preload("res://breakout/scenes/skills/basic/basic_skill.tres"),
	preload("res://breakout/scenes/skills/blood_bullet/blood_bullet.tres"),
]

@onready var skill_scene_pool: Array[PackedScene] = [
	preload("res://breakout/scenes/skills/basic/basic_skill.tscn"),
	preload("res://breakout/scenes/skills/blood_bullet/blood_bullet.tscn"),
]


@onready var void_level_name: String = "void level"
@onready var void_level_scene = preload("res://breakout/scenes/levels/void_level.tscn")

@onready var level_name_pool: Array[String] = [
	"test level 0",
	"test level 99_o_",
]

@onready var level_scene_pool: Array[PackedScene] = [
	preload("res://breakout/scenes/levels/test_level_0.tscn"),
	preload("res://breakout/scenes/levels/test_level_99_o_.tscn"),
]

# please ensure one-to-one correspondence of tres_pool and scene_pool

###

var each_drop_percent: Array[float] = []
var each_lucky_drop_percent: Array[float] = []

var id_to_drop_scene_index: Array[int] = []
var name_to_drop_scene_index: Array[String] = []

var id_to_consumable_scene_index: Array[int] = []
var name_to_consumable_scene_index: Array[String] = []

var id_to_skill_scene_index: Array[int] = []
var name_to_skill_scene_index: Array[String] = []

var name_to_level_scene_index: Array[String] = []


func _ready():
	_get_drop_percent()
	_get_lucky_drop_percent()
	
	_get_id_to_drop_scene_index()
	_get_name_to_drop_scene_index()
	
	_get_id_to_consumable_scene_index()
	_get_name_to_consumable_scene_index()
	
	_get_id_to_skill_scene_index()
	_get_name_to_skill_scene_index()
	
	_get_name_to_level_scene_index()

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
	var index = id_to_drop_scene_index.find(drop_id)
	if index == -1:
		return void_drop_scene
	else:
		return drop_scene_pool[index]

func get_drop_scene_by_name(drop_name: String) -> PackedScene:
	var index = name_to_drop_scene_index.find(drop_name)
	if index == -1:
		return void_drop_scene
	else:
		return drop_scene_pool[index]


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
	
	
func spawn_unique_drop(global_pos: Vector2, drop_name: String):
	var drop_scene_instance = get_drop_scene_by_name(drop_name).instantiate() as Drop
	var drops = get_tree().get_first_node_in_group("drops") as Node2D
	drops.add_child(drop_scene_instance)
	drop_scene_instance.global_position = global_pos


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
	
	
func _get_id_to_consumable_scene_index():
	id_to_consumable_scene_index.clear()
	for consumable_tres in (consumable_tres_pool as Array[Consumable_Info]):
		id_to_consumable_scene_index.append(consumable_tres.consumable_id)
	
func _get_name_to_consumable_scene_index():
	name_to_consumable_scene_index.clear()
	for consumable_tres in (consumable_tres_pool as Array[Consumable_Info]):
		name_to_consumable_scene_index.append(consumable_tres.consumable_name)

func get_consumable_scene_by_id(consumable_id: int) -> PackedScene:
	var index = id_to_consumable_scene_index.find(consumable_id)
	if index == -1:
		return void_consumable_scene
	else:
		return consumable_scene_pool[index]

func get_consumable_scene_by_name(consumable_name: String) -> PackedScene:
	var index = name_to_consumable_scene_index.find(consumable_name)
	if index == -1:
		return void_consumable_scene
	else:
		return consumable_scene_pool[index]
	
	
func _get_id_to_skill_scene_index():
	id_to_skill_scene_index.clear()
	for skill_tres in (skill_tres_pool as Array[Skill_Info]):
		id_to_skill_scene_index.append(skill_tres.skill_id)
	
func _get_name_to_skill_scene_index():
	name_to_skill_scene_index.clear()
	for skill_tres in (skill_tres_pool as Array[Skill_Info]):
		name_to_skill_scene_index.append(skill_tres.skill_name)

func get_skill_scene_by_id(skill_id: int) -> PackedScene:
	var index = id_to_skill_scene_index.find(skill_id)
	if index == -1:
		return void_skill_scene
	else:
		return skill_scene_pool[index]

func get_skill_scene_by_name(skill_name: String) -> PackedScene:
	print(skill_name, name_to_skill_scene_index)
	var index = name_to_skill_scene_index.find(skill_name)
	if index == -1:
		return void_skill_scene
	else:
		return skill_scene_pool[index]
	
	
func _get_name_to_level_scene_index():
	name_to_level_scene_index.clear()
	for level_name in (level_name_pool as Array[String]):
		name_to_level_scene_index.append(level_name)

func get_level_scene_by_name(level_name: String) -> PackedScene:
	var index = name_to_level_scene_index.find(level_name)
	if index == -1:
		return null
	else:
		return level_scene_pool[index]
