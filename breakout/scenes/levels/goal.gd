extends Node
class_name Goal

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


# general func
func get_bricks():
	return $Bricks.get_children()

func get_enemies():
	return $Enemies.get_children()
	
func get_drops():
	return $Drops.get_children()

func load_bricks_to_node(bricks: Node):
	for brick in self.get_bricks():
		brick.get_parent().remove_child(brick)
		bricks.call_deferred("add_child", brick)
		
func load_enemies_to_node(enemies: Node):
	for enemy in self.get_enemies():
		enemy.get_parent().remove_child(enemy)
		enemies.call_deferred("add_child", enemy)
		
func load_drops_to_node(drops: Node):
	for drop in self.get_drops():
		drop.get_parent().remove_child(drop)
		drops.call_deferred("add_child", drop)

func load_all_to_level(level: Node):
	load_bricks_to_node(level.get_node("./Bricks"))
	load_enemies_to_node(level.get_node("./Enemies"))
	load_drops_to_node(level.get_node("./Drops"))
