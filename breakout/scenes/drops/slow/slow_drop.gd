extends Drop

func apply_effect():
	var balls = get_tree().get_first_node_in_group("balls") as Balls
	balls.change_all_ball_speed(drop_info.effect_multiplier)
	#TODO: 需要做一个视觉效果
