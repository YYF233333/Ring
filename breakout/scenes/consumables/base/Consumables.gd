extends Control
class_name Consumables

@onready var select_box = $SelectBox as TextureRect
@onready var grid_container = $GridContainer

var consumable_num: int
var selected_consumable_num: int
var consumable_nodes: Array[Node] #小心这里Node都是索引


# Called when the node enters the scene tree for the first time.
func _ready():
	BreakoutManager.consumables = self
	self.consumable_nodes = grid_container.get_children()
	selected_consumable_num = 0
	update()
	

func _process(delta):
	if !consumable_num:
		return
		
	if Input.is_action_just_pressed("select_next_consumable"):
		if consumable_num > 1:
			selected_consumable_num = (selected_consumable_num + 1) % consumable_num
			move_select_box()
	if Input.is_action_just_pressed("use_selected_consumable"):
		var selected_consumable = consumable_nodes[selected_consumable_num] as Consumable
		if selected_consumable.use(1):
			select_box_flicker()
	
		
func reset():
	selected_consumable_num = 0
	update()
		

func move_select_box():
	var tween = create_tween()
	tween.tween_property(select_box, "global_position", consumable_nodes[selected_consumable_num].global_position, 0.5).set_trans(Tween.TRANS_EXPO).set_ease(Tween.EASE_OUT)
		
func select_box_flicker():
	var tween = create_tween()
	tween.tween_property(select_box, "modulate", Color(1, 1, 1, 0), 0.1).set_trans(Tween.TRANS_BOUNCE)
	tween.parallel().tween_property(select_box, "scale", Vector2(0.9, 0.9), 0.1).set_trans(Tween.TRANS_BOUNCE)
	tween.chain()
	tween.tween_property(select_box, "modulate", Color(1, 1, 1, 1), 0.1).set_trans(Tween.TRANS_BOUNCE)
	tween.parallel().tween_property(select_box, "scale", Vector2(1, 1), 0.1).set_trans(Tween.TRANS_BOUNCE)
	
func update():
	consumable_num = consumable_nodes.size()
	if consumable_num == 0:
		select_box.modulate = Color(1, 1, 1, 0)
	else:
		var tween = create_tween()
		tween.tween_property(select_box, "global_position", consumable_nodes[selected_consumable_num].global_position, 0.5).set_trans(Tween.TRANS_EXPO).set_ease(Tween.EASE_OUT)
		tween.parallel().tween_property(select_box, "modulate", Color(1, 1, 1, 1), 0.5).set_trans(Tween.TRANS_BOUNCE)
		

func add_consumable_instance(consumable: Consumable, node: Node = grid_container):
	var scene_file_path = consumable.scene_file_path
	var consumable_instance = load(scene_file_path).instantiate() as Consumable
	node.add_child(consumable_instance)
	#consumable_instance.apply_passive_effect()


func remove_consumable_instance(consumable: Consumable, node: Node = grid_container):
	for child in node.get_children():
		if child.consumable_info.consumable_id == consumable.consumable_info.consumable_id:
			#child.remove_passive_effect()
			node.remove_child(child)
			child.queue_free()
			break

func change_consumable_instance(from: Consumable, to: Consumable, node: Node = grid_container):
	if !from:
		add_consumable_instance(node, to)
	else:
		remove_consumable_instance(node, from)
		add_consumable_instance(node, to)
		
func find_consumable_by_id(id: int):
	for consumable in consumable_nodes:
		if consumable.consumable_info.consumable_id == id:
			return consumable
	return null
	
func find_consumable_by_name(name: String):
	for consumable in consumable_nodes:
		if consumable.consumable_info.consumable_name == name:
			return consumable
	return null
	
func _on_consumable_manager_consumable_change(from: Consumable, to: Consumable):
	change_consumable_instance(from, to)
	update()

func _on_consumable_manager_consumable_restore(consumable: Consumable, quantity: int):
	for consumable_i in consumable_nodes:
		if consumable_i.consumable_info.consumable_id == consumable.consumable_id:
			consumable_i.restore_charge(1)
