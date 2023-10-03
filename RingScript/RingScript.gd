class_name RingScript
## 脚本编译器



static func compile(source: String) -> Array[InstBlock]:
	var inst_blocks: Array[InstBlock] = []
	var parser := Parser.new(source)
	while not parser.end():
		var block := parser.next_block()
		if block != null:
			inst_blocks.append(block)
	return inst_blocks
