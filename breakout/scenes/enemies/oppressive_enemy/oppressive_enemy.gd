extends Enemy

	
func check_init_type():
	#debug
	# check if init_type correspond with other stats
	if type != 0:
		print("type seems not correct")
	
func update_stat(delta):
	if !knock_back_timer.time_left:
		current_speed = move_toward(current_speed, basic_speed, basic_acceleration * delta)
		current_direction = get_direction_to_paddle("bottom") as Vector2
	velocity = current_direction * current_speed
