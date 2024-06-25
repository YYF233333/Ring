extends Line2D

@export var limited_lifetime := true
@export var min_spwan_distance := 5.0
@export var max_point_num := 25

var tick_speed := 0.1
var tick := 0.0

func _ready():
	clear_points()

func insert_point(pos: Vector2):
	if get_point_count() > 0 and pos.distance_to(points[get_point_count()-1]) < min_spwan_distance:
		return
	add_point(pos)
	if get_point_count() > max_point_num:
		for i in range(get_point_count() - max_point_num):
			remove_point(0)
