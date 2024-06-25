extends Enemy

	
func check_init_type():
	#debug
	# check if init_type correspond with other stats
	if type != 0:
		print("type seems not correct")
	
func update_stat(delta):
	current_speed = move_toward(current_speed, basic_speed, basic_acceleration * delta)
	if !knock_back_timer.time_left:
		direction = get_direction_to_paddle("bottom")
		velocity = direction.normalized() * current_speed
	else:
		velocity = direction.normalized() * velocity

