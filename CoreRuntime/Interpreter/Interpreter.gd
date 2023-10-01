extends Node
## 全局脚本解释器

const InstBlock = RingScript.InstBlock

## 脚本执行环境
var stage: Stage
var asset_server: Node
var save_server: Node
var cur_script: Array[InstBlock]
var next_block_idx := 0

func _ready() -> void:
	stage = Stage
	asset_server = AssetServer
	save_server = SaveServer

func load_script(script: Array[InstBlock]) -> void:
	cur_script = script

func step() -> void:
	var cur_block := cur_script[next_block_idx]
	for cur_inst in cur_block.insts:
		cur_inst.execute([], self)
		if cur_inst.has_execute_failed():
			push_error("Error: %s" % cur_inst.get_error_text())
	next_block_idx += 1

## 脚本可调用接口
## 接口返回值统一为解释器本身
func example() -> void:
	print("test user-script interface")

func say(name: String, content: String):
	print("%s say %s" % [name, content])
