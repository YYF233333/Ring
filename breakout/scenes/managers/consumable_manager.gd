extends Node

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

# please ensure one-to-one correspondence of consumable_tres_pool and consumable_scene_pool

var id_to_consumable_scene_index: Array[int] = []
var name_to_consumable_scene_index: Array[String] = []


func _ready():
	_get_id_to_consumable_scene_index()
	_get_name_to_consumable_scene_index()
		
func _get_id_to_consumable_scene_index():
	id_to_consumable_scene_index.clear()
	for consumable_tres in (consumable_tres_pool as Array[Consumable_Info]):
		id_to_consumable_scene_index.append(consumable_tres.consumable_id)
	
func _get_name_to_consumable_scene_index():
	name_to_consumable_scene_index.clear()
	for consumable_tres in (consumable_tres_pool as Array[Consumable_Info]):
		name_to_consumable_scene_index.append(consumable_tres.consumable_name)

func get_consumable_scene_by_id(consumable_id: int) -> PackedScene:
	return consumable_scene_pool[id_to_consumable_scene_index.find(consumable_id)]

func get_consumable_scene_by_name(consumable_name: String) -> PackedScene:
	return consumable_scene_pool[name_to_consumable_scene_index.find(consumable_name)]
