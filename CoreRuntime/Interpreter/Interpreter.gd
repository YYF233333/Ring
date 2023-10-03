extends Node
## 全局脚本解释器

# 脚本执行环境
var stage: Stage
var asset_server: Node
var save_server: Node
var cur_script: Array[InstBlock] = []
var next_block_idx := 0
var local_variables: Dictionary = {}
var stack: Array = []

func _ready() -> void:
	stage = Stage
	asset_server = AssetServer
	save_server = SaveServer

func load_script(script: Array[InstBlock]) -> void:
	cur_script = script

func step() -> void:
	var cur_block := cur_script[next_block_idx]
	for cur_inst in cur_block.insts:
		# Interpreter is Env
		cur_inst.execute([], self)
		if cur_inst.has_execute_failed():
			push_error("Error: %s" % cur_inst.get_error_text())
	next_block_idx += 1

## 脚本可调用接口

func example() -> void:
	print("test user-script interface")

func say(name: String, content: String):
	print("%s say %s" % [name, content])

func show_character(name: String) -> void:
	stage.characters["红叶"] = Character.new(["红叶"], [load(name)])
	stage.add_child(stage.characters["红叶"])

# 局部变量管理
func push_stack(item) -> void:
	stack.push_back(item)

func pop_stack() -> Variant:
	return stack.pop_back()

func set_local(name: String, value: Variant) -> void:
	local_variables[name] = value

func get_local(name: String) -> Variant:
	if not local_variables.has(name):
		push_error("Accessing unexisted local var %s", name)
	return local_variables.get(name)

func remove_local(name: String) -> bool:
	return local_variables.erase(name)
