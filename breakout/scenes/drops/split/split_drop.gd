extends Drop

func apply_effect():
	var balls = get_tree().get_first_node_in_group("balls") as Balls
	balls.all_split()
	
	#test
	#await get_tree().create_timer(2.0).timeout
	#balls.all_split()
	#
	#await get_tree().create_timer(2.0).timeout
	#balls.all_split()
	#
	#await get_tree().create_timer(2.0).timeout
	#balls.all_split()
	#
	#await get_tree().create_timer(2.0).timeout
	#balls.all_split()
	#
	#await get_tree().create_timer(2.0).timeout
	#balls.all_split()
	#
	#await get_tree().create_timer(2.0).timeout
	#balls.all_split()
