extends Node2D
class_name DeathComponent

#TODO: 多种掉落物随机抽
@export_enum("basic_enemy","basic_brick"," ") var death_type: String = " "

@export var health_component: HealthComponent
@export var sprite_2d: Sprite2D

@onready var animation_player: AnimationPlayer = $AnimationPlayer
@onready var pop_gpu_particles_2d: GPUParticles2D = $PopGPUParticles2D

# Called when the node enters the scene tree for the first time.
func _ready():
	pop_gpu_particles_2d.texture = sprite_2d.texture
	health_component.died.connect(_on_died)
	
func spawn_unique_drop(spawn_global_position: Vector2):
	ResourceManager.spawn_unique_drop(spawn_global_position, owner.unique_drop_name)

func spawn_standard_drop(spawn_global_position: Vector2):
	ResourceManager.spawn_random_drop(spawn_global_position)

func spawn_lucky_drop(spawn_global_position: Vector2):
	ResourceManager.spawn_random_lucky_drop(spawn_global_position)

func spawn_drop(spawn_global_position: Vector2):
	if owner.lucky_drop:
		if owner.unique_drop_name:
			spawn_unique_drop(spawn_global_position)
		else:
			spawn_lucky_drop(spawn_global_position)
	elif randf() <= ValueManager.current_drop_percent * owner.drop_percent_scale:
		if owner.unique_drop_name:
			spawn_unique_drop(spawn_global_position) # 想稳定掉落指定drop且不闪光，将drop percent调成100就行
		else:
			spawn_standard_drop(spawn_global_position)
			
func _on_died():
	BreakoutManager.point_scored.emit(owner.point)
	
	var spawn_global_position = (owner as Node2D).global_position
	
	spawn_drop(spawn_global_position)
	
	_spawn_particles(spawn_global_position)
			
func _spawn_particles(spawn_global_position: Vector2):
	var foregrounds = get_tree().get_first_node_in_group("foregrounds")
	get_parent().remove_child(self)
	foregrounds.add_child(self)
	global_position = spawn_global_position
	
	match death_type:
		"basic_enemy":
			animation_player.play("pop")
		"basic_brick":
			pass
		_:
			pass
	
