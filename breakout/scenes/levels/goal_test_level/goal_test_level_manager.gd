extends Node

#signal BreakoutManager.goal_clear
#signal BreakoutManager.level_clear
const level_name = "goal_test_level"
const goal_description = [
					"start level",
					"try to shoot a ball",
					"1: ball shot",
					"try to break a brick",
					"2: brick broken", 
					"try to beat an enemy",
					"3: enemy beaten",
					"plenty of enemies appear",
					"try to use your skill",
					"4: all cleared",
				]
const GOAL_2 = preload("res://breakout/scenes/levels/goal_test_level/goal_2.tscn")		
@onready var goal_2 = GOAL_2.instantiate(PackedScene.GEN_EDIT_STATE_MAIN_INHERITED) as Goal

@onready var level: Node2D = $"../Level"
@onready var bricks: Node2D = $"../Level/Bricks"
@onready var enemies: Node2D = $"../Level/Enemies"
@onready var drops: Node2D = $"../Level/Drops"

var current_goal_id = 0


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	BreakoutManager.ammo_lost.connect(_on_ammo_lost)
	BreakoutManager.brick_broken.connect(_on_brick_broken)
	BreakoutManager.enemy_beaten.connect(_on_enemy_beaten)
	call_deferred("init_level")
	
func _on_ammo_lost(value: int): #tip：nnd不加value: int識別不到
	if current_goal_id == 1:
		end_goal_1()
		
func _on_brick_broken(brick_name: StringName):
	if current_goal_id == 2:
		end_goal_2()
	
func _on_enemy_beaten(enemy_name: StringName):
	if current_goal_id == 3:
		end_goal_3()

func init_level():
	BreakoutManager.ammo = 99
	BreakoutManager.pause_game()
	await get_tree().create_timer(1).timeout
	BreakoutManager.continue_game()
	start_goal_1()

func start_goal_1():
	current_goal_id = 1
	#BreakoutManager.pause_game()
	#await Utility.dialogue("press [UpArrow] or [W] to shoot a ball")
	#BreakoutManager.continue_game()
	#Utility.dialogue("press [UpArrow] or [W] to shoot a ball")
	BreakoutManager.goal_text_changed.emit("press [UpArrow] or [W] to shoot a ball")
	
func end_goal_1():
	BreakoutManager.goal_clear.emit(level_name, "ball shot")
	start_goal_2()
	
func start_goal_2():
	goal_2.load_all_to_level(level)
	current_goal_id = 2
	BreakoutManager.goal_text_changed.emit("protect the ball from falling out and break a brick")
	
func end_goal_2():
	BreakoutManager.goal_clear.emit(level_name, "brick broken")
	start_goal_3()

func start_goal_3():
	current_goal_id = 3
	BreakoutManager.goal_text_changed.emit("beat the enemy")
	
func end_goal_3():
	BreakoutManager.goal_clear.emit(level_name, "enemy beaten")
	BreakoutManager.level_clear.emit(level_name)
	start_goal_4()

func start_goal_4():
	pass
	
func end_goal_4():
	BreakoutManager.goal_clear.emit(level_name, "10s survived")
	start_goal_5()

func start_goal_5():
	pass
	
func end_goal_5():
	BreakoutManager.goal_clear.emit(level_name, "enemies cleared")
	start_goal_6()

func start_goal_6():
	pass
	
func end_goal_6():
	BreakoutManager.level_clear.emit(level_name)
