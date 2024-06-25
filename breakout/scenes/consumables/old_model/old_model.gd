extends Consumable

#debug
@export var transform_time = 30.0

var transformed = false:
	set(value):
		transformed = value
		if !value:
			consumable_info = preload("res://breakout/scenes/consumables/old_model/old_model.tres")
			texture_normal = preload("res://breakout/assets/Mine/old model.png")
		else:
			consumable_info = preload("res://breakout/scenes/consumables/old_model/new_model.tres")
			texture_normal = preload("res://breakout/assets/Mine/new model.png")
			
func _init_info():
	max_possess_amount = consumable_info.max_possess_amount
	price = consumable_info.price
	transformed = ValueManager.old_model_transformed
	#debug
	rest_times = 99
	pass
			
func apply_effect():
	if !transformed:
		transform()
	else:
		apply_rebound()
	return true

func transform(time: float = transform_time):
	BreakoutManager.cursed = true
	
	await get_tree().create_timer(time).timeout
	
	BreakoutManager.cursed = false
	transformed = true
	restore(1)

func apply_rebound(time: float = transform_time):
	BreakoutManager.rebound = true
	
	await get_tree().create_timer(time).timeout
	
	BreakoutManager.rebound = false
	

