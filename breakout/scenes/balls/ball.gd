extends CharacterBody2D
class_name Ball

# onready
@onready var fire_ball_timer = $FireBallTimer as Timer
@onready var lightning_ball_timer = $LightningBallTimer as Timer
@onready var sprite = $Sprite2D as Sprite2D

@onready var collision_shape = $CollisionShape2D as CollisionShape2D

#stat
@export var init_speed : float = ValueManager.ball_init_speed
var speed_scale : float = 1.0

var current_speed : float = 1.0:
	set(value):
		current_speed = max(value, 1.0)
		velocity = velocity.normalized() * current_speed

# buff
var fire_ball_buff: int = 0:
	set(value):
		fire_ball_buff = max(value, 0)
		update_sprite()

var lightning_ball_buff: int = 0:
	set(value):
		lightning_ball_buff = max(value, 0)
		update_sprite()



func _ready():
	current_speed = init_speed
	randomize_direction()
	
	fire_ball_timer.timeout.connect(_on_fire_ball_timer_timeout)
	lightning_ball_timer.timeout.connect(_on_lightning_ball_timer_timeout)
	
	add_to_group("balls")


func _process(delta):
	if $Line2D.tick > $Line2D.tick_speed:
		$Line2D.insert_point(global_position)
	else:
		$Line2D.tick += delta


func _physics_process(delta):
	var collision = move_and_collide(velocity * delta)
	var collider:Node2D = collision.get_collider() if collision else null
	
	if collider:
		if collider.is_in_group("paddles"):
			bounce_off_paddle(collider)
		elif collider.is_in_group("walls"):
			bounce_off_walls(collision)
		elif collider.is_in_group("bricks"):
			bounce_off_bricks(collision, collider)
		elif collider.is_in_group("enemies"):
			bounce_off_enemies(collision, collider)
		else:
			print_debug("what's this?")


func bounce_off_walls(wall : KinematicCollision2D):
	velocity = velocity.bounce(wall.get_normal())
	
func bounce_off_bricks(brick : KinematicCollision2D, collider : Brick):
	velocity = velocity.bounce(brick.get_normal())
	
func bounce_off_enemies(collision : KinematicCollision2D, collider : Enemy):
	velocity = velocity.bounce(collision.get_normal())
		
func bounce_off_paddle(paddle : Paddle):
	var paddle_position = paddle.global_position
	var direction_to_ball = paddle_position.direction_to(global_position)
	
	var current_direction = velocity.normalized()
	var updated_direction = (direction_to_ball - current_direction).normalized()
	
	velocity = updated_direction * current_speed


func randomize_direction():
	var random_direction = Vector2()
	
	while is_zero_approx(random_direction.x) or is_zero_approx(random_direction.y):
		random_direction.x = randf_range(-1.0, 1.0)
		random_direction.y = -1.0

	random_direction = random_direction.normalized()
	velocity = random_direction * current_speed
	
	
func change_fire_ball_buff(layer: int = 1, last_time: float = 5.0):
	fire_ball_buff += layer
	
	if fire_ball_buff > 0:
		fire_ball_timer.wait_time = last_time
		fire_ball_timer.start()
		
		
func change_lightning_ball_buff(layer: int = 1, last_time: float = 5.0):
	lightning_ball_buff += layer
	
	if lightning_ball_buff > 0:
		lightning_ball_timer.wait_time = last_time
		lightning_ball_timer.start()
	

func update_sprite():
	var red_rate = (fire_ball_buff + 3.0) / (fire_ball_buff + lightning_ball_buff + 3.0)
	var green_rate = 3.0 / (fire_ball_buff + lightning_ball_buff + 3.0)
	var blue_rate = (lightning_ball_buff + 3.0) / (fire_ball_buff + lightning_ball_buff + 3.0)
	modulate = Color(red_rate, green_rate, blue_rate, 1.0)
	#TODO: 有buff时的视觉效果，用shader？particle？毕竟可能和其他效果叠加
	pass
	

#func broken():
	#remove_from_group("balls")
	#$CollisionShape2D.disabled = true
	#shard_emitter.shatter()
	#await shard_emitter.delete_timer.timeout
	#queue_free()
	
func _on_fire_ball_timer_timeout():
	change_fire_ball_buff(-1, fire_ball_timer.wait_time)
	
func _on_lightning_ball_timer_timeout():
	change_lightning_ball_buff(-1, lightning_ball_timer.wait_time)
	
