class_name BGNode
extends StatementNode

var path: String ## path to the new background image
var animation: String ## animation used in show process

func parse(source: String) -> String:
	var rule := RegEx.create_from_string(
		"!\\[(?<anim>[\\s\\S]*\\)]\\((?<path>[\\s\\S]*)\\)"
		)
	var matched := rule.search(source)
	if matched == null or matched.get_start() != 0:
		return source
	path = matched.get_string("path")
	path = path.strip_edges()
	animation = matched.get_string("anim")
	animation = animation.strip_edges()
	source = source.substr(matched.get_end())
	return source
