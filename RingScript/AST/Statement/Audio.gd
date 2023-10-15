class_name AudioNode
extends StatementNode

var path: String ## path to the audio file("" excluded)
var animation: String ## animation used in audio change

func parse(source: String) -> String:
	var rule := RegEx.create_from_string(
		"<audio src=\"(?<path>[\\s\\S]*)\"></audio>"
		)
	var matched := rule.search(source)
	if matched == null or matched.get_start() != 0:
		return source
	path = matched.get_string("path")
	source = source.substr(matched.get_end())
	source = source.strip_edges(true, false)
	if source.begins_with("with"):
		source = source.trim_prefix("with")
		source = source.strip_edges(true, false)
		var ret := Lexer.parse_identifier(source)
		source = ret[1]
		animation = ret[0]
	return source
