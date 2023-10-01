class_name RingScript
## 脚本编译器

class InstBlock:
	var insts: Array[Expression]

	func _init(insts: Array[Expression]) -> void:
		self.insts = insts

	func append(other: InstBlock) -> void:
		insts.append_array(other.insts)

class Parser:
	var source: String
	var index: int

	func _init(source: String) -> void:
		self.source = source
		index = 0

	func end() -> bool:
		return index >= source.length()

	func next_block() -> InstBlock:
		if index < source.length():
			var ret := parse_say()
			if ret != null:
				return InstBlock.new([ret])
			ret = parse_bg_change()
			if ret != null:
				return InstBlock.new([ret])
			parse_any()
		return null

	func parse_say() -> Expression:
		var say := RegEx.create_from_string("(?<name>[\\S]+):\\\"(?<text>[^\\\"]+)\\\"")
		var say_block := say.search(source, index)
		if say_block != null and say_block.get_start() == index:
			index = say_block.get_end()
			var expr_text := "say(\"%s\", \"%s\")" % \
			[say_block.get_string("name"), say_block.get_string("text")]
			var expr := Expression.new()
			var ec := expr.parse(expr_text)
			if ec != Error.OK:
				push_error("Invalid syntax: %s" % say_block.get_string())
			else:
				return expr
		return null

	func parse_bg_change() -> Expression:
		var bg_change := RegEx.create_from_string("!\\[\\]\\((?<img_path>[\\s\\S]*)\\)")
		var bg_change_block := bg_change.search(source, index)
		if bg_change_block != null and bg_change_block.get_start() == index:
			index = bg_change_block.get_end()
			var expr_text := "stage.background.show_picture(\"%s\")" \
			% bg_change_block.get_string("img_path")
			print(expr_text)
			var expr := Expression.new()
			var ec := expr.parse(expr_text)
			if ec != Error.OK:
				push_error("Invalid syntax: %s" % bg_change_block.get_string())
			else:
				return expr
		return null

	func parse_code() -> Expression:
		return Expression.new()

	func parse_any() -> void:
		push_warning("Unmatched character %s" % source[index])
		index += 1

static func compile(source: String) -> Array[InstBlock]:
	var inst_blocks: Array[InstBlock] = []
	var parser := Parser.new(source)
	while not parser.end():
		var block := parser.next_block()
		if block != null:
			inst_blocks.append(block)
	return inst_blocks

#static func parse(source: String) -> Array[InstBlock]:
#	var ret: Array[InstBlock] = []
#	var code_block := RegEx.create_from_string("```[\\S]*\\n(?<code>[\\s\\S]*)```")
