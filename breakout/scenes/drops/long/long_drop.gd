extends Drop

func apply_effect():
	var paddle = get_tree().get_first_node_in_group("paddles") as Paddle
	paddle.change_length_buff(drop_info.effect_layer, drop_info.effect_last_time)
	#paddle.change_speed_buff(-drop_info.effect_layer, drop_info.effect_last_time)
	#TODO: 需要做一个视觉效果
