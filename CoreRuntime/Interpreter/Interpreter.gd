extends Node
## 全局脚本解释器

## 脚本执行环境
var stage: Stage
var asset_server: Node
var save_server: Node
var cur_script: RingScript
var cur_inst_idx := -1

func _ready() -> void:
	stage = Stage.new()
	asset_server = AssetServer
	save_server = SaveServer

func load_script(script: RingScript) -> void:
	cur_script = script

func step() -> void:
	cur_inst_idx += 1
	var cur_inst := cur_script.instructions[cur_inst_idx]
	cur_inst.execute([], self)
	if cur_inst.has_execute_failed():
		push_error("Error: %s" % cur_inst.get_error_text())

## 脚本可调用接口
## 接口返回值统一为解释器本身
func example() -> void:
	print("test user-script interface")
