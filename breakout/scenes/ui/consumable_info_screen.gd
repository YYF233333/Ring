extends PanelContainer

class_name ConsumableInfoScreen

@export var consumable: Consumable

@onready var sprite: TextureRect = $MarginContainer/VBoxContainer/HBoxContainer/Sprite
@onready var rest_times_label: Label = $MarginContainer/VBoxContainer/HBoxContainer/Sprite/RestTimesLabel
@onready var name_label: Label = $MarginContainer/VBoxContainer/HBoxContainer/NameLabel
@onready var description_label: Label = $MarginContainer/VBoxContainer/DescriptionLabel

func _ready() -> void:
	update()

func hint_not_available():
	if consumable.rest_times == 0:
		Utility.hint_not_available(rest_times_label)
	else:
		Utility.hint_not_available(description_label)

func update():
	if !consumable:
		sprite.set("texture", null)
		rest_times_label.text = str(0)
		name_label.text = ""
		description_label.text = ""
	else:
		sprite.set("texture", consumable.texture_normal)
		rest_times_label.text = str(consumable.rest_times)
		name_label.text = consumable.consumable_info.consumable_name
		description_label.text = consumable.consumable_info.consumable_description
