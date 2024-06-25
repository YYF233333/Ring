extends TextureButton
class_name Consumable

@export var consumable_info: Consumable_Info

@onready var rest_times_label = $RestTimesLabel as Label

var max_possess_amount: int
var price: int
var rest_times: int:
	set(value):
		rest_times = min(max(value, 0), max_possess_amount)
		rest_times_label.text = str(rest_times)
		
		
func _ready():
	_init_info()
	_connect_signal()

func _init_info():
	max_possess_amount = consumable_info.max_possess_amount
	price = consumable_info.price
	#debug
	rest_times = 99
	pass
	
func _connect_signal():
	pass

func _process(delta):
	update()

func update():
	pass


func use(num: int = 1):
	if !check_usable(num):
		Utility.hint_not_available(self)
		return false
		
	#Effect here
	if apply_effect():
		exhaust(num)
		return true
	else:
		Utility.hint_not_available(self)
		return false
		
func check_usable(value: int):
	if rest_times >= value:
		return true
	else:
		return false

func apply_effect():
	BreakoutManager.point_scored.emit(1)
	print("void used")
	return true
	
func exhaust(value: int):
	rest_times = min(max(rest_times-value, 0), consumable_info.max_possess_amount)

func restore(value: int):
	if rest_times == consumable_info.max_possess_amount:
		return false
	else:
		rest_times = min(rest_times+value, consumable_info.max_possess_amount)
		return true


#func shift_label_mode(mode: int):
	#match mode:
		#SHIFT_MODE.SHOP:
			#$CurrentCharge/Charge.hide()
			#$CurrentCharge/Price.show()
			#$CurrentCharge/budget.show()
		#SHIFT_MODE.PLAY:
			#$CurrentCharge/Charge.show()
			#$CurrentCharge/Price.hide()
			#$CurrentCharge/budget.hide()


