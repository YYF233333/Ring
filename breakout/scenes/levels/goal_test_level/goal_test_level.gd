extends Node2D
class_name GOAL_TEST_LEVEL

#signal BreakoutManager.goal_clear
#signal BreakoutManager.level_clear
const level_name = "example_level"
const goal_list = [
					"start level",
					"try to shoot a ball",
					"1: ball shot",
					"try to break a brick",
					"2: brick broken", 
					"try to beat an enemy",
					"3: enemy beaten",
					"plenty of enemies and bricks appear",
					"try to survive for 10s",
					"4: 10s survived",
					"try to use a skill",
					"5: enemies cleared",
					"try to clear all bricks",
					"6: all cleared",
				]
				
var current_goal_id = 0


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	BreakoutManager.ammo_lost.connect(_on_ammo_lost)
	BreakoutManager.brick_broken.connect(_on_brick_broken)
	call_deferred("init_level")
	
func _on_brick_broken(brick_name: StringName):
	if current_goal_id == 2:
		end_goal_2()
	
func _on_ammo_lost(value: int): #tip：nnd不加value: int識別不到
	if current_goal_id == 1:
		end_goal_1()

func init_level():
	BreakoutManager.ammo = 99
	BreakoutManager.pause_game()
	await get_tree().create_timer(1).timeout
	BreakoutManager.continue_game()
	start_goal_1()

func start_goal_1():
	current_goal_id = 1
	
func end_goal_1():
	BreakoutManager.goal_clear.emit(level_name, "ball shot")
	BreakoutManager.pause_game()
	await get_tree().create_timer(1).timeout
	BreakoutManager.continue_game()
	start_goal_2()
	
func start_goal_2():
	current_goal_id = 2
	
func end_goal_2():
	BreakoutManager.goal_clear.emit(level_name, "brick broken")
	BreakoutManager.pause_game()
	await get_tree().create_timer(1).timeout
	BreakoutManager.continue_game()
	start_goal_3()

func start_goal_3():
	current_goal_id = 3
	
func end_goal_3():
	BreakoutManager.goal_clear.emit(level_name, "enemy beaten")
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


# general func

func get_bricks():
	return $Bricks.get_children()

func get_enemies():
	return $Enemies.get_children()
