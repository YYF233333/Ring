class_name HideNode
extends StatementNode

var identifier: String ## name of the Object to hide
var animation: String ## animation used in hide process

func parse(source: String) -> String:
	if not source.begins_with("hide"):
		return source
	source = source.trim_prefix("hide")
	source = source.strip_edges(true, false)
	var ret := Lexer.parse_identifier(source)
	source = ret[1]
	identifier = ret[0]
	source = source.strip_edges(true, false)
	if source.begins_with("with"):
		source = source.trim_prefix("with")
		source = source.strip_edges(true, false)
		ret = Lexer.parse_identifier(source)
		source = ret[1]
		animation = ret[0]
	return source
