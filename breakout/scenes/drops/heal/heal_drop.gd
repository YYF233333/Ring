extends Drop

var heal_percent: float = 0.25
var heal: int

func apply_effect():
	if BreakoutManager.current_health >= 1:
		heal = ceili(BreakoutManager.max_health * heal_percent)
		BreakoutManager.take_damage(-heal)
	
