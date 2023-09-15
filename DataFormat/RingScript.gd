class_name RingScript
## 存放编译完成的脚本，可直接被解释器执行

class InstBlock:
	var insts: Array[Expression]

## 编译后指令
var instructions: Array[Expression]

func _init(source: String) -> void:
	instructions = parse(source)

static func parse(source: String) -> Array[Expression]:
	var ret: Array[Expression] = []
	var regex := RegEx.new()
	regex.compile(
		"(?<name>[\\S]+):\\\"(?<text>[^\\\"]+)\\\"" \
		+ "|```[\\S]*\\n(?<code>[\\s\\S]*)```" \
		+ "|!\\[\\]\\((?<img_path>[\\s\\S]*)\\)"
	)
	var inst := regex.search(source)
	while inst:
		var expr_text := ""
		var expr := Expression.new()
		if inst.get_string("name"):
			expr_text = "%s.say(%s)" % \
			[inst.get_string("name"), inst.get_string("text")]
		elif inst.get_string("code"):
			expr_text = inst.get_string("code")
		elif inst.get_string("img_path"):
			expr_text = "bg.show_texture(%s)" % inst.get_string("img_path")
		else:
			push_error("Unmatched script part %s" % inst.get_string())
		var ec := expr.parse(expr_text)
		if ec != Error.OK:
			push_error("Invalid syntax: %s" % expr_text)
		else:
			ret.push_back(expr)
		inst = regex.search(source, inst.get_end())
	return ret
