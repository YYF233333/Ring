extends Node2D

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
					"5 enemies cleared",
					"try to clear all bricks",
					"6 all cleared",
				]

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	start_goal_1()

func start_goal_1():
	pass
	
func end_goal_1():
	BreakoutManager.goal_clear.emit(level_name, "ball shot")
	start_goal_2()
	
func start_goal_2():
	pass
