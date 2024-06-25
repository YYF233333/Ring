extends StaticBody2D
class_name StaticWall

@onready var collision_shape = $CollisionShape2D as CollisionShape2D
@onready var segment_shape = collision_shape.shape as SegmentShape2D

func _ready():
	add_to_group("walls")
