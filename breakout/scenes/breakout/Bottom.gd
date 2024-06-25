extends Area2D

func _ready():
	body_entered.connect(_on_body_entered)

func _on_body_entered(body):
	if body is Ball:
		BreakoutManager.ball_lost.emit(body)
		# TODO: 球删除
