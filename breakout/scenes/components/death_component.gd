extends Node
class_name DeathComponent

#TODO: 多种掉落物随机抽
@export var health_component: HealthComponent

# Called when the node enters the scene tree for the first time.
func _ready():
	health_component.died.connect(_on_died)
	
func spawn_unique_drop():
	var spawn_global_position = (owner as Node2D).global_position
	ResourceManager.spawn_unique_drop(spawn_global_position, owner.unique_drop_name)

func spawn_drop():
	var spawn_global_position = (owner as Node2D).global_position
	ResourceManager.spawn_random_drop(spawn_global_position)

func spawn_lucky_drop():
	var spawn_global_position = (owner as Node2D).global_position
	ResourceManager.spawn_random_lucky_drop(spawn_global_position)

func _on_died():
	BreakoutManager.point_scored.emit(owner.point)
	
	if owner.lucky_drop:
		if owner.unique_drop_name:
			spawn_unique_drop()
		else:
			spawn_lucky_drop()
	elif randf() <= ValueManager.current_drop_percent * owner.drop_percent_scale:
		if owner.unique_drop_name:
			spawn_unique_drop() # 想稳定掉落指定drop且不闪光，将drop percent调成100就行
		else:
			spawn_drop()
	
