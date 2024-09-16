extends Control
class_name Consumables

@onready var grid_container = $GridContainer
@onready var select_box: TextureRect = $SelectBoxAnchor/SelectBox
@onready var select_box_anchor: Control = $SelectBoxAnchor # 不然scale的时候位于左上角的position会变
@onready var animation_player: AnimationPlayer = $AnimationPlayer
@onready var consumable_info_screen: ConsumableInfoScreen = $ConsumableInfoScreen

var consumable_num: int = 0
var selected_consumable_num: int = 0
var consumable_nodes: Array[Node] #小心这里Node都是索引


# Called when the node enters the scene tree for the first time.
func _ready():
	BreakoutManager.consumables = self
	self.consumable_nodes = grid_container.get_children()
	update()
	

func _process(delta):
	if !consumable_num:
		return
		
	if Input.is_action_just_pressed("select_next_consumable"):
		if consumable_num > 1:
			selected_consumable_num = (selected_consumable_num + 1) % consumable_num
			move_select_box()
		#update()
	if Input.is_action_just_pressed("use_selected_consumable"):
		var selected_consumable = consumable_nodes[selected_consumable_num] as Consumable
		if selected_consumable.use(1):
			select_box_flicker()
		else:
			consumable_info_screen.hint_not_available()
		#update()
	
		
func reset(clear_consumable: bool = true, node: Node = grid_container):
	if clear_consumable:
		for consumable in consumable_nodes:
			node.remove_child(consumable)
			consumable.queue_free()
	selected_consumable_num = 0
	update()
		

func move_select_box():
	var tween = create_tween()
	tween.tween_property(select_box_anchor, "global_position", consumable_nodes[selected_consumable_num].global_position, 0.5).set_trans(Tween.TRANS_EXPO).set_ease(Tween.EASE_OUT)
	
		
func select_box_flicker():
	animation_player.play("RESET")
	animation_player.play("flicker")


func update():
	"""
	将consumable_nodes与grid_container同步
	以及更新select_box的位置
	
	请在consumable_nodes有改变时
	"""
	var old_consumable_num = consumable_num
	consumable_nodes = grid_container.get_children()
	consumable_num = consumable_nodes.size()
	
	# select_box vanish or appear on consumable_num changing
	if old_consumable_num > 0 && consumable_num == 0: # varnish
		select_box.modulate = Color(1, 1, 1, 0)
		select_box_anchor.global_position = Vector2(-1280, -720)
	elif old_consumable_num == 0 && consumable_num > 0: # appear
		var tween = create_tween()
		tween.tween_property(select_box_anchor, "global_position", consumable_nodes[selected_consumable_num].global_position, 0.5).set_trans(Tween.TRANS_EXPO).set_ease(Tween.EASE_OUT)
		tween.parallel().tween_property(select_box, "modulate", Color(1, 1, 1, 1), 0.5).set_trans(Tween.TRANS_BOUNCE)
	else:
		pass
		

	# update info screen
	if consumable_num == 0:
		consumable_info_screen.consumable = null
		consumable_info_screen.update()
	else:
		consumable_info_screen.consumable = consumable_nodes[selected_consumable_num]
		consumable_info_screen.update()
		
	


func add_consumable_instance(consumable: Consumable, node: Node = grid_container):
	var consumable_scene_file_path = consumable.scene_file_path
	var consumable_instance = load(consumable_scene_file_path).instantiate() as Consumable
	node.add_child(consumable_instance)
	#consumable_instance.apply_passive_effect()
	update()


func remove_consumable_instance(consumable: Consumable, node: Node = grid_container):
	for child in node.get_children():
		if child.consumable_info.consumable_id == consumable.consumable_info.consumable_id:
			#child.remove_passive_effect()
			node.remove_child(child)
			child.queue_free()
			break
	update()


func change_consumable_instance(from: Consumable, to: Consumable, node: Node = grid_container):
	if !from:
		add_consumable_instance(node, to)
	else:
		remove_consumable_instance(node, from)
		add_consumable_instance(node, to)
	update()
		
		
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


func _on_consumable_manager_consumable_restore(consumable: Consumable, value: int = 1):
	for consumable_i in consumable_nodes:
		if consumable_i.consumable_info.consumable_id == consumable.consumable_id:
			consumable_i.restore_charge(value)
	update()
