extends RayCast2D
class_name LaserBeam

@onready var line2D = $Line2D as Line2D
@onready var casting_particle = $CastingParticle as GPUParticles2D
@onready var colliding_particle = $CollidingParticle as GPUParticles2D
@onready var beam_particle = $BeamParticle as GPUParticles2D

@onready var attack_cool_down_timer = $AttackCoolDownTimer as Timer

var tween : Tween:
	set(value):
		if tween:
			tween.kill()
		tween = value
		
var is_casting = false:
	set(value):
		is_casting = value
		
		casting_particle.emitting = is_casting
		beam_particle.emitting = is_casting
		if is_casting:
			appear()
		else:
			colliding_particle.emitting = false
			disappear()
		set_physics_process(is_casting)

#var level: int = 0:
	#set(value):
		#level = value
		#
		#if level == 0:
			#is_casting = false
		#else:
			#if !is_casting:
				#is_casting = true
#
#var damage: float

var target

		
func _input(event):
	if event is InputEventMouseButton:
		is_casting = event.pressed
		
func _ready():
	attack_cool_down_timer.timeout.connect(_on_attack_cool_down_timer_timeout)
	
	set_physics_process(false)
	line2D.points[1] = Vector2.ZERO

func _physics_process(delta):
	var cast_point = target_position
	force_raycast_update()
	
	colliding_particle.emitting = is_colliding()
	
	if is_colliding():
		cast_point = to_local(get_collision_point())
		colliding_particle.global_rotation = get_collision_normal().angle()
		colliding_particle.position = cast_point
		
		target = get_collider()
	else:
		target = null
	
	line2D.points[1] = cast_point
	beam_particle.position = cast_point * 0.5
	(beam_particle.process_material as ParticleProcessMaterial).emission_box_extents.x = cast_point.length() * 0.5


func appear():
	tween = create_tween().set_parallel(true)
	tween.tween_property(line2D, "width", 10.0, 0.2)
	attack_cool_down_timer.start()
	
func disappear():
	tween = create_tween().set_parallel(true)
	tween.tween_property(line2D, "width", 0.0, 0.2)
	attack_cool_down_timer.stop()

func _on_attack_cool_down_timer_timeout():
	if target:
		if target is Enemy:
			target.hit_by_laser_beam(self)
		elif target is Brick:
			target.hit_by_laser_beam(self)
