extends Consumable


func apply_effect():
	ValueManager.ball_damage_change(0.0, 4.0)
	return true


