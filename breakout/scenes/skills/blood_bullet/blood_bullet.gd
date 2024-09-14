extends Skill

func apply_effect():
	if BreakoutManager.current_health > 1:
		BreakoutManager.take_damage(1)
		(BreakoutManager.balls as Balls).shoot_new_ball(false, 0)
		return true #返回false就不会消耗charge
	else:
		return false
