class_name AlphaAnimation
extends RingAnimation

var duration: float
var start: float
var end: float

func _init(duration: float, end := 1.0, start := 0.0) -> void:
	self.duration = duration
	self.end = end
	self.start = start
		
func apply(tween: Tween, node: Node) -> Tween:
	node.modulate.a = start
	var final = node.modulate
	final.a = end
	tween.tween_property(node, "modulate", final, duration)
	return tween
