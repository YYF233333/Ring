extends Consumable


var transformed = false:
	set(value):
		transformed = value
		if !value:
			consumable_info = preload("res://breakout/scenes/consumables/package/notebook.tres")
			texture_normal = preload("res://breakout/assets/Mine/package.png")
		else:
			consumable_info = preload("res://breakout/scenes/consumables/package/package.tres")
			texture_normal = preload("res://breakout/assets/Mine/notebook.png")

func _init_info():
	max_possess_amount = consumable_info.max_possess_amount
	price = consumable_info.price
	transformed = ValueManager.package_transformed
	#debug
	rest_times = 99
	pass

func apply_effect():
	if !transformed:
		return false
	else:
		apply_bless()
	return true

func apply_bless():
	BreakoutManager.blessed = true
	ValueManager.player_charge_multiplier *= 2.0
	ValueManager.ball_damage_multiplier += 1.0
	BreakoutManager.ammo += 5
	BreakoutManager.current_health = BreakoutManager.max_health
