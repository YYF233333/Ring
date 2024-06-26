extends Node
class_name Skill_Manager

@onready var void_skill_tres = preload("res://breakout/scenes/consumables/base/consumable.tres")
@onready var void_skill_scene = preload("res://breakout/scenes/consumables/base/consumable.tscn")

@onready var skill_tres_pool: Array[Resource] = [
]

@onready var skill_scene_pool: Array[PackedScene] = [
]
