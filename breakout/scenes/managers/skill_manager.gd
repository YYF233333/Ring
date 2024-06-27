extends Node

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

# please ensure one-to-one correspondence of skill_tres_pool and skill_scene_pool

var id_to_skill_scene_index: Array[int] = []
var name_to_skill_scene_index: Array[String] = []


func _ready():
	_get_id_to_skill_scene_index()
	_get_name_to_skill_scene_index()
		
func _get_id_to_skill_scene_index():
	id_to_skill_scene_index.clear()
	for skill_tres in (skill_tres_pool as Array[Skill_Info]):
		id_to_skill_scene_index.append(skill_tres.skill_id)
	
func _get_name_to_skill_scene_index():
	name_to_skill_scene_index.clear()
	for skill_tres in (skill_tres_pool as Array[Skill_Info]):
		name_to_skill_scene_index.append(skill_tres.skill_name)

func get_skill_scene_by_id(skill_id: int) -> PackedScene:
	return skill_scene_pool[id_to_skill_scene_index.find(skill_id)]

func get_skill_scene_by_name(skill_name: String) -> PackedScene:
	return skill_scene_pool[name_to_skill_scene_index.find(skill_name)]
