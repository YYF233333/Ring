extends TextureButton
class_name Skill

@export var skill_info: Skill_Info

var current_charge: int:
	set(value):
		current_charge = max(value, 0)
		BreakoutManager.skill_charged.emit()
var exhaust: int
var max_charge: int:
	set(value):
		max_charge = max(value, 0)
		BreakoutManager.skill_charged.emit()

func apply_effect():
	return false #返回false就不会消耗charge

func _ready():
	_init_info()
	_connect_signal()

func _init_info():
	current_charge = 0 # TODO: 以后的current_charge重置再考虑
	exhaust = skill_info.exhaust
	max_charge = skill_info.max_charge
	
	$PriceLabel.text = "%d" % skill_info.price
	
	BreakoutManager.skill = self
	
func _connect_signal():
	self.pressed.connect(_on_pressed)
	BreakoutManager.ball_hit.connect(_on_ball_hit)
	BreakoutManager.skill_charge.connect(_on_skill_charge)
	
func _process(delta):
	if Input.is_action_just_pressed("use_skill"):
		BreakoutManager.try_use_skill.emit()
		
func try_use():
	if !use(exhaust):
		Utility.hint_not_available(self)
	
func check_usable(value: int):
	if current_charge >= value:
		return true
	else:
		return false

func reset():
	current_charge = 0

func use(exhaust_value: int = exhaust):
	if !check_usable(exhaust_value):
		return false
	
	#Effect here
	if apply_effect():
		exhaust_charge(exhaust_value)
		return true
	else:
		return false


func exhaust_charge(value: int):
	if current_charge <= 0:
		return false
	else:
		current_charge = max(current_charge-value, 0)
		return true


func restore_charge(value: int):
	if current_charge >= max_charge:
		return false
	else:
		current_charge = min(current_charge+value, max_charge)
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

func _on_pressed():
	BreakoutManager.try_use_skill.emit()
	
func _on_ball_hit(charge: int):
	_on_skill_charge(charge)
	
func _on_skill_charge(value: int):
	value = ceil(value * ValueManager.player_charge_multiplier)
	restore_charge(value)
