extends Drop

var charge_percent: float = 0.5
var charge: int

func apply_effect():
	if BreakoutManager.skill:
		charge = ceili(BreakoutManager.skill.max_charge * charge_percent)
		BreakoutManager.skill_charge.emit(charge)
