extends Node

@onready var noise = FastNoiseLite.new()


func hint_not_available(node: Node):
	flicker_red(node)
	
	#var shake_tween = create_tween().set_loops(2).parallel()
	#shake_tween.tween_property(self, "position", Vector2(0,0), 0.4).as_relative().set_trans(Tween.TRANS_SPRING)
	# TODO：想做抖动，但这样做好像不行
	# 高频调用shake()？
	
	
func flicker_red(node: Node, times: int = 2, color: Color = Color(0.66, 0, 0, 0.66)):
	var flicker_tween = create_tween().set_loops(times)
	flicker_tween.tween_property(node, "modulate", color, 0.1)
	flicker_tween.tween_property(node, "modulate", Color(1, 1, 1, 1), 0.1)
	return flicker_tween
	
	
func flicker_transparent(node: Node, times: int = 2, color: Color = Color(1, 1, 1, 0.33)):
	var flicker_tween = create_tween().set_loops(times)
	flicker_tween.tween_property(node, "modulate", color, 0.5)
	flicker_tween.tween_property(node, "modulate", Color(1, 1, 1, 1), 0.5)
	return flicker_tween
	
	
func screen_shake(shake_force: float = 1.0):
	BreakoutManager.breakout.shake = true
	BreakoutManager.breakout.shake_force = shake_force
	
	await get_tree().create_timer(0.1).timeout
	
	BreakoutManager.breakout.shake = false
	
	
func wait_with_end_hint(node: Node, time: float, end_hint_time: float = 5.0):
	if time > end_hint_time:
		await get_tree().create_timer(time-end_hint_time).timeout
		if node:
			end_hint(node, end_hint_time)
	else:
		if node:
			end_hint(node, time)


func end_hint(node: Node, last_time: float):
	var flicker_tween = flicker_transparent(node, -1) as Tween
	await get_tree().create_timer(last_time).timeout
	flicker_tween.kill()
	
	
