extends Drop

func apply_effect():
	var paddle = get_tree().get_first_node_in_group("paddles") as Paddle
	paddle.shoot_laser_beam(drop_info.effect_last_time)
