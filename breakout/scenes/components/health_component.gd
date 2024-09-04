extends Node
class_name HealthComponent

signal health_change #use to update health_bar
signal died

@export var max_health: int = 4
@export var init_health: int = 4
var current_health: int


# Called when the node enters the scene tree for the first time.
func _ready():
	if (owner is Brick) || (owner is Enemy):
		pass
	else:
		#debug
		print("health component has wrong owner")
		queue_free()
		
	current_health = init_health
	
	
func take_damage(value: int):
	#debug
	current_health = min(max(current_health - value, 0), max_health)
	health_change.emit()
	Callable(check_death).call_deferred()
	
	
func set_max_health(value: int):
	max_health = max(value, 0)
	if current_health > max_health:
		current_health = max_health
	health_change.emit()
	Callable(check_death).call_deferred()
	
	
func get_health_percent():
	if max_health <= 0:
		return 0
	else:
		return min(current_health/float(max_health), 1)
			
		
func check_death():
	if current_health == 0:
		died.emit()
		#owner.queue_free()
