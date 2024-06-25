extends Control


func _ready():
	BreakoutManager.point_scored.connect(_on_point_scored)
	BreakoutManager.ammo_changed.connect(_on_ammo_changed)
	BreakoutManager.skill_charged.connect(_on_skill_charged)
	BreakoutManager.health_changed.connect(_on_health_changed)
	
	BreakoutManager.reset()

func _on_point_scored(_point):
	$Hud/Score.text = str(BreakoutManager.score)


func _on_ammo_changed():
	$Hud/Ammo.text = "Ammo: " + str(BreakoutManager.ammo)
	

func _on_skill_charged():
	$Status/Skill/ChargeBar.value = BreakoutManager.get_charge_percent()
	$Status/Skill/ChargeBar/ChargeInfo.text = "%d/%d" % [BreakoutManager.skill.current_charge, BreakoutManager.skill.max_charge]

func _on_health_changed(value: int):
	$Status/Health/HealthBar.value = BreakoutManager.get_health_percent()
	$Status/Health/HealthBar/HealthInfo.text = "%d/%d" % [BreakoutManager.current_health, BreakoutManager.max_health]
