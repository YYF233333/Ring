extends Node
class_name IntervalHurtComponent

@export var health_component: HealthComponent

# timer
@onready var fire_hurt_timer = $FireHurtTimer as Timer

# debuff
var fire_debuff_layer: int = 0:
	set(value):
		if fire_debuff_layer == 0 && value > 0:
			fire_hurt_timer.wait_time = ValueManager.fire_hurt_interval
			fire_hurt_timer.start()
		fire_debuff_layer = max(value, 0)
		if fire_debuff_layer == 0:
			fire_hurt_timer.stop()
		update_sprite()


func _ready():
	fire_hurt_timer.timeout.connect(_on_fire_hurt_timer_timeout)
	
func update_sprite():
	var blue_rate = 3.0 / (fire_debuff_layer + 3.0)
	owner.modulate = Color(1.0, blue_rate, blue_rate, 1.0)
	#TODO: 改变贴图，用shader？particle？毕竟可能和其他效果叠加
	
func _on_fire_hurt_timer_timeout():
	health_component.take_damage(fire_debuff_layer)
	fire_debuff_layer -= 1
