extends Consumable


func apply_effect():
	(BreakoutManager.balls as Balls).shoot_new_ball(false, 0)
	return true



