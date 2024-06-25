extends Node
class_name Consumable_Manager

#@onready var void_consumable_tres = preload("res://breakout/scenes/consumables/base/consumable.tres")
@onready var void_consumable_scene = preload("res://breakout/scenes/consumables/base/consumable.tscn")

@onready var consumable_tres_pool: Array[Resource] = [
]

@onready var consumable_scene_pool: Array[PackedScene] = [
]

# please ensure one-to-one correspondence of consumable_tres_pool and consumable_scene_pool

var each_consumable_percent: Array[float] = []

var id_to_consumable_scene_index: Array[int] = []
var name_to_consumable_scene_index: Array[String] = []


func _ready():
	_get_consumable_percent()
	_get_id_to_consumable_scene_index()


func _get_consumable_percent():
	each_consumable_percent.clear()
	var percent_sum = 0.0
	for consumable_tres in (consumable_tres_pool as Array[Drop_Info]):
		var current_consumable_percent = pow(2.0, -consumable_tres.basic_consumable_luck_level)
		each_consumable_percent.append(current_consumable_percent)
		percent_sum += current_consumable_percent
	each_consumable_percent.assign(each_consumable_percent.map(func(num):
		return num / percent_sum
	))
		
		
func _get_id_to_consumable_scene_index():
	id_to_consumable_scene_index.clear()
	for consumable_tres in (consumable_tres_pool as Array[Drop_Info]):
		id_to_consumable_scene_index.append(consumable_tres.consumable_id)
	
func _get_name_to_consumable_scene_index():
	name_to_consumable_scene_index.clear()
	for consumable_tres in (consumable_tres_pool as Array[Drop_Info]):
		name_to_consumable_scene_index.append(consumable_tres.consumable_name)

func get_consumable_scene_by_id(consumable_id: int) -> PackedScene:
	return consumable_scene_pool[id_to_consumable_scene_index.find(consumable_id)]

func get_consumable_scene_by_name(consumable_name: String) -> PackedScene:
	return consumable_scene_pool[name_to_consumable_scene_index.find(consumable_name)]


func get_random_consumable_scene(each_consumable_percent: Array[float] = self.each_consumable_percent) -> PackedScene:
	var random_number = randf()
	var current_percent = 0
	for i in range(each_consumable_percent.size()):
		current_percent += each_consumable_percent[i]
		if random_number < current_percent:
			return consumable_scene_pool[i]
	return void_consumable_scene
	

func spawn_random_consumable(global_pos: Vector2, each_consumable_percent: Array[float] = self.each_consumable_percent):
	var random_consumable_scene_instance = get_random_consumable_scene(each_consumable_percent).instantiate() as Drop
	var consumables = get_tree().get_first_node_in_group("consumables") as Node2D
	consumables.add_child(random_consumable_scene_instance)
	random_consumable_scene_instance.global_position = global_pos
