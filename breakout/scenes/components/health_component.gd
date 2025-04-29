extends Node
class_name HealthComponent

signal health_change #use to update health_bar
signal died

var current_health: int

@export var show_floating_text: bool = true
var floating_text_scene = preload("res://breakout/scenes/ui/floating_text.tscn")

# Called when the node enters the scene tree for the first time.
func _ready():
	if (owner is Brick) || (owner is Enemy):
		pass
	else:
		#debug
		print_debug("health component has wrong owner")
		queue_free()
		
	current_health = owner.init_health
	
	
func take_damage(value: int):
	#debug
	var prev_health = current_health
	
	current_health = min(max(current_health - value, 0), owner.max_health)
	health_change.emit()
	
	if show_floating_text:
		#加载伤害 UI 组件
		var floating_text = floating_text_scene.instantiate() as FloatingText
		get_tree().get_first_node_in_group("foregrounds").add_child(floating_text)
		
		floating_text.global_position = owner.global_position + \
			Vector2(randi_range(-8,8),randi_range(-4,4)-16)
		
		var health_change_value = current_health - prev_health
		if health_change_value == 0:
			pass
		elif health_change_value > 0:
			floating_text.start(str(abs(health_change_value)), Color(0.0, 1.0, 0.0))
		elif health_change_value < 0:
			floating_text.start(str(abs(health_change_value)), Color(1.0, 0.0, 0.0))
	
	Callable(check_death).call_deferred()


func set_max_health(value: int):
	owner.max_health = max(value, 0)
	if current_health > owner.max_health:
		current_health = owner.max_health
	health_change.emit()
	Callable(check_death).call_deferred()
	
	
func get_health_percent():
	if owner.max_health <= 0:
		return 0
	else:
		return min(current_health/float(owner.max_health), 1)
			
		
func check_death():
	if current_health == 0:
		died.emit()
