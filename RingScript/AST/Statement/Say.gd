class_name SayNode
extends StatementNode

var character_name: String
var say_content: String

func parse(source: String) -> String:
	var ret := Lexer.parse_identifier(source)
	character_name = ret[0]
	if character_name.ends_with(":") or character_name.ends_with("："):
		character_name = character_name.trim_suffix(":")
		character_name = character_name.trim_suffix("：")
		source = ret[1]
		source = source.strip_edges(true, false)
		ret = source.split("\n", true, 1)
		say_content = ret[0]
		source = ret[1]
	return source
