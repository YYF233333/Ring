class_name Parser

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
			return ret
		ret = parse_bg_change()
		if ret != null:
			return ret
		ret = parse_code()
		if ret != null:
			return ret
		ret = parse_show_character()
		if ret != null:
			return ret
		ret = parse_hide_character()
		if ret != null:
			return ret
		parse_any()
	return null

func parse_say() -> InstBlock:
	var rule := RegEx.create_from_string("(?<name>[\\S]+):\\\"(?<text>[^\\\"]+)\\\"")
	var matched := rule.search(source, index)
	if matched == null or matched.get_start() != index:
		return null
	index = matched.get_end()
	var expr_text := "say(\"%s\", \"%s\")" % \
	[matched.get_string("name"), matched.get_string("text")]
	var expr := Expression.new()
	var ec := expr.parse(expr_text)
	if ec != Error.OK:
		push_error("Invalid syntax: %s" % matched.get_string())
		return null
	return InstBlock.new([expr])

func parse_bg_change() -> InstBlock:
	var rule := RegEx.create_from_string("!\\[\\]\\((?<img_path>[\\s\\S]*)\\)")
	var matched := rule.search(source, index)
	if matched == null or matched.get_start() != index:
		return null
	index = matched.get_end()
	var expr_text := "stage.background.show_picture(\
		asset_server.load_img(\"%s\"))" \
		% ("res://" + matched.get_string("img_path"))
	var expr := Expression.new()
	var ec := expr.parse(expr_text)
	if ec != Error.OK:
		push_error("Invalid syntax: %s" % matched.get_string())
		return null
	return InstBlock.new([expr])

func parse_show_character() -> InstBlock:
	var rule := RegEx.create_from_string("show (?<name>[\\S]*) at (?<pos>[\\S]*)\\n")
	var matched := rule.search(source, index)
	if matched == null or matched.get_start() != index:
		return null
	index = matched.get_end()
	var text := "show_character(\"%s\", \"%s\")" % \
		[matched.get_string("name"), matched.get_string("pos")]
	var expr := Expression.new()
	var ec := expr.parse(text)
	if ec != Error.OK:
		push_error("Invalid Syntax: %s" % matched.get_string())
		return null
	return InstBlock.new([expr])

func parse_hide_character() -> InstBlock:
	var rule := RegEx.create_from_string("hide (?<name>[\\S]*)\\n")
	var matched := rule.search(source, index)
	if matched == null or matched.get_start() != index:
		return null
	index = matched.get_end()
	var text := "hide_character(\"%s\")" % matched.get_string("name")
	var expr := Expression.new()
	var ec := expr.parse(text)
	if ec != Error.OK:
		push_error("Invalid Syntax: %s" % matched.get_string())
		return null
	return InstBlock.new([expr])

func parse_code() -> InstBlock:
	var rule := RegEx.create_from_string("```[\\S]*\\n(?<code>[\\s\\S]*)```")
	var matched := rule.search(source, index)
	if matched == null or matched.get_start() != index:
		return null
	index = matched.get_end()
	var lines := matched.get_string("code")
	var block := InstBlock.new()
	for line in lines.split("\n", false):
		# 去掉注释
		line = line.get_slice("#", 0)
		if line.length() == 0:
			continue
		var expr := Expression.new()
		var ec := expr.parse(line)
		if ec != Error.OK:
			push_error("Invalid syntax: %s" % line)
			return null
		block.push(expr)
	return block

func parse_any() -> void:
	push_warning("Unmatched character %s" % source[index])
	index += 1
