extends Node2D
class_name Balls

@export var ball_scenes: PackedScene
var	exist_ball_quantity: int = 0
var ball_quantity_limit: int = ValueManager.ball_quantity_limit

func _ready():
	BreakoutManager.balls = self

func _process(delta):
	if Input.is_action_just_pressed("shoot"):
		shoot_new_ball()

	exist_ball_quantity = get_child_count()
	if exist_ball_quantity <= 0 && BreakoutManager.ammo > 0:
		shoot_new_ball()

func instantiate_new_ball():
	var new_ball = ball_scenes.instantiate() as Ball
	return new_ball


func add_ball(ball: Ball = null):
	if !ball:
		return false
		
	add_child(ball)
	return true


func del_ball(ball: Ball):
	if ball:
		ball.queue_free()
	
#func break_ball(ball: Ball):
	#if ball:
		#ball.broken()
		
		
func shoot_new_ball(del_old_ball: bool = !BreakoutManager.debug, ammo_exhaust: int = 1):
	if del_old_ball:
		if BreakoutManager.ammo < ammo_exhaust:
			print_debug("ammo not enough") #TODO: ammo label flicker
			return
			
		var exist_balls = get_children() as Array[Ball]
		for ball in exist_balls:
			del_ball(ball)
			
	BreakoutManager.ammo_lost.emit(ammo_exhaust)
	var paddle = get_tree().get_first_node_in_group("paddles") as Paddle
	var new_ball = instantiate_new_ball() as Ball
	add_ball(new_ball)
	var offset := Vector2(0.0, new_ball.collision_shape.shape.radius * new_ball.scale.y
		+ paddle.shape.size.y * paddle.scale.y / 2.0 + 10.0)
	#TODO:
	new_ball.global_position = paddle.global_position - offset
		
		
func split_ball(ball: Ball, num: int = ValueManager.ball_split_quantity) -> bool:
	#exist_ball_quantity = get_child_count()
	#num = min(ball_quantity_limit - (exist_ball_quantity - 1), num) 
	#if num == 1:
		#del_ball(ball)
		#return false
	# 不在此进行limit判断，请调用方自行判断
	if !ball:
		return false
	
	for i in range(num):
		#var start = Time.get_ticks_usec()
		var child_ball = instantiate_new_ball()
		#var mid = Time.get_ticks_usec()
		if add_ball(child_ball):
			child_ball.global_position = ball.global_position
			child_ball.velocity = ball.velocity.rotated(((num-1)/2.0-i)*PI/(4.0*num))
			
			child_ball.init_speed = ball.init_speed
			child_ball.current_speed = ball.current_speed
			
			child_ball.fire_ball_buff = ball.fire_ball_buff
			child_ball.fire_ball_timer.wait_time = ball.fire_ball_timer.wait_time
			child_ball.fire_ball_timer.start()
			
			child_ball.lightning_ball_buff = ball.lightning_ball_buff
			child_ball.lightning_ball_timer.wait_time = ball.lightning_ball_timer.wait_time
			child_ball.lightning_ball_timer.start()
			#var end = Time.get_ticks_usec()
			#print_debug("%d %d %d" % [end-start, mid-start, end-mid])
			
	#remove_child(ball)
	#del_ball(ball)
	# 这里也请调用方自行删除
	return true

func all_split(num: int = ValueManager.ball_split_quantity):
	exist_ball_quantity = get_child_count()
	var ball_spawn_limit = ball_quantity_limit - exist_ball_quantity
	if ball_spawn_limit == 0:
		print("ball quantity reach the limit!") #TODO: label hint
		return
	var exist_balls = get_children() as Array[Ball]
	exist_balls.shuffle() # ensure the latest ball to split first
	
	var full_split_ball_quantity = min(int(ball_spawn_limit / num), exist_ball_quantity)
	var left_ball_split_num = ball_spawn_limit - full_split_ball_quantity * num
	
	# full split ball
	for i in range(full_split_ball_quantity):
		call_deferred("split_ball", exist_balls[i], num)
		#split_ball(exist_balls[i], num)
		call_deferred("del_ball", exist_balls[i])
	
	# partly split ball
	if left_ball_split_num > 0 && exist_ball_quantity > full_split_ball_quantity:
		print("ball quantity reach the limit!") #TODO: label hint
		call_deferred("split_ball", exist_balls[full_split_ball_quantity], left_ball_split_num)
		#split_ball(exist_balls[full_split_ball_quantity + 1], last_ball_split_num)
		call_deferred("del_ball", exist_balls[full_split_ball_quantity])


func change_single_ball_speed(ball: Ball, multiplier: float):
	if !ball:
		return
		
	ball.current_speed *= multiplier


func change_all_ball_speed(multiplier: float):
	var exist_balls = get_children() as Array[Ball]
	for ball in exist_balls:
		change_single_ball_speed(ball, multiplier)
		

func fire_all_ball(layer: int = 1, last_time: float = 5.0):
	var exist_balls = get_children() as Array[Ball]
	for ball in exist_balls:
		ball.change_fire_ball_buff(layer, last_time)


func lightning_all_ball(layer: int = 1, last_time: float = 5.0):
	var exist_balls = get_children() as Array[Ball]
	for ball in exist_balls:
		ball.change_lightning_ball_buff(layer, last_time)
