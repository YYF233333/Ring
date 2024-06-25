extends Brick


func check_init_type():
	#debug
	if type != min(health_component.current_health-1, 1):
		print("type seems not correct")
		

func physic_hurt(value: int):
	if value > 999:
		health_component.take_damage(health_component.current_health-1)
		#TODO: strong_break visual effect
	else:
		health_component.take_damage(min(value,1))


func _on_health_change():
	var current_health = health_component.current_health
	if current_health > 0:
		set_type(min(current_health-1, 1))
		show_health()
