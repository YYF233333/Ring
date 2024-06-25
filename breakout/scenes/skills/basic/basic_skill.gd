extends Skill

func apply_effect():
	(BreakoutManager.balls as Balls).all_split()
	(BreakoutManager.paddle as Paddle).change_length_buff(99, 20.0)
	return true

