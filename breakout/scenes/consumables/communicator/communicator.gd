extends Consumable
		

func apply_effect():
	BreakoutManager.skill_charge.emit(10)
	return true
