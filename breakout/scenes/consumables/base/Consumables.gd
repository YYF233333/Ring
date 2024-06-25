extends Control

@onready var select_box = $SelectBox as TextureRect

var consumable_num: int
var selected_consumable_num: int
var consumables: Array[Node]


# Called when the node enters the scene tree for the first time.
func _ready():
	get_info()
	

func _process(delta):
	if !consumable_num:
		return
		
	if Input.is_action_just_pressed("select_next_consumable"):
		if consumable_num > 1:
			selected_consumable_num = (selected_consumable_num + 1) % consumable_num
			move_select_box()
	if Input.is_action_just_pressed("use_selected_consumable"):
		var selected_consumable = consumables[selected_consumable_num] as Consumable
		if selected_consumable.use(1):
			select_box_flicker()
			
	#手柄玩家好像没这么多按键
	#if Input.is_action_just_pressed("use_consumable_1"):
		#var selected_consumable = consumables[0] as Consumable
		#selected_consumable.use(selected_consumable.exhaust)
	#if Input.is_action_just_pressed("use_consumable_2"):
		#var selected_consumable = consumables[1] as Consumable
		#selected_consumable.use(selected_consumable.exhaust)
	#if Input.is_action_just_pressed("use_consumable_3"):
		#var selected_consumable = consumables[2] as Consumable
		#selected_consumable.use(selected_consumable.exhaust)
	#if Input.is_action_just_pressed("use_consumable_4"):
		#var selected_consumable = consumables[3] as Consumable
		#selected_consumable.use(selected_consumable.exhaust)

func move_select_box():
	var tween = create_tween()
	tween.tween_property(select_box, "global_position", consumables[selected_consumable_num].global_position, 0.5).set_trans(Tween.TRANS_EXPO).set_ease(Tween.EASE_OUT)
		
func select_box_flicker():
	var tween = create_tween()
	tween.tween_property(select_box, "modulate", Color(1, 1, 1, 0), 0.1).set_trans(Tween.TRANS_BOUNCE)
	tween.parallel().tween_property(select_box, "scale", Vector2(0.9, 0.9), 0.1).set_trans(Tween.TRANS_BOUNCE)
	tween.chain()
	tween.tween_property(select_box, "modulate", Color(1, 1, 1, 1), 0.1).set_trans(Tween.TRANS_BOUNCE)
	tween.parallel().tween_property(select_box, "scale", Vector2(1, 1), 0.1).set_trans(Tween.TRANS_BOUNCE)
	

func add_consumable_instance(node: Node, consumable: Consumable):
	var scene_file_path = consumable.scene_file_path
	var consumable_instance = load(scene_file_path).instantiate() as Consumable
	node.add_child(consumable_instance)
	consumable_instance.apply_passive_effect()


func remove_consumable_instance(node: Node, consumable: Consumable):
	for child in node.get_children():
		if child.consumable.consumable_id == consumable.consumable_id:
			child.remove_passive_effect()
			node.remove_child(child)
			child.queue_free()
			break

func change_consumable_instance(node: Node, from: Consumable, to: Consumable):
	if !from:
		add_consumable_instance(node, to)
	else:
		remove_consumable_instance(node, from)
		add_consumable_instance(node, to)

func get_info():
	selected_consumable_num = 0
	consumables = $GridContainer.get_children()
	consumable_num = consumables.size()
	if consumable_num == 0:
		select_box.modulate = Color(1, 1, 1, 0)
	else:
		var tween = create_tween()
		tween.tween_property(select_box, "global_position", consumables[selected_consumable_num].global_position, 0.5).set_trans(Tween.TRANS_EXPO).set_ease(Tween.EASE_OUT)
		tween.parallel().tween_property(select_box, "modulate", Color(1, 1, 1, 1), 0.5).set_trans(Tween.TRANS_BOUNCE)

func _on_consumable_manager_consumable_change(from: Consumable, to: Consumable):
	change_consumable_instance($Consumable, from, to)
	get_info()

func _on_consumable_manager_consumable_restore(consumable: Consumable, quantity: int):
	for consumable_i in consumables:
		if consumable_i.consumable_info.consumable_id == consumable.consumable_id:
			consumable_i.restore_charge(1)
