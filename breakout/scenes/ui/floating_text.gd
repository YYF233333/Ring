extends Node2D

class_name FloatingText

@onready var label: Label = $Label

func _ready():
	pass

func start(text: String, color: Color = Color(0.0, 0.0, 0.0), direction: Vector2 = Vector2.UP):
	label.text = text
	label.set("theme_override_colors/font_color", color)
	
	var tween = create_tween()
	tween.set_parallel()
	tween.tween_property(self, "global_position", global_position + (direction * 16), 0.3)\
	.set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_CUBIC)
	tween.chain()
	
	tween.tween_property(self, "global_position", global_position + (direction * 48), 0.5)\
	.set_ease(Tween.EASE_IN).set_trans(Tween.TRANS_CUBIC)
	tween.chain()
	
	var scale_tween = create_tween()
	
	scale_tween.tween_property(
		self, 'scale', Vector2.ONE * 1.5, 0.15
	).set_ease(
		Tween.EASE_OUT
	).set_trans(
		Tween.TRANS_CUBIC
	)
	
	scale_tween.tween_property(
		self, 'scale', Vector2.ONE, 0.15
	).set_ease(
		Tween.EASE_IN
	).set_trans(
		Tween.TRANS_CUBIC
	)
	
	#伤害飘出后，自动销毁，否则会一直在屏幕上显示
	tween.tween_callback(queue_free)
