class_name ShowNode
extends StatementNode

var identifier: String ## name of the Object to show
var position: String ## where the Object should be placed
var animation: String ## animation used in show process

func parse(source: String) -> String:
	if not source.begins_with("show"):
		return source
	source = source.trim_prefix("show")
	source = source.strip_edges(true, false)
	var ret := Lexer.parse_identifier(source)
	source = ret[1]
	identifier = ret[0]
	source = source.strip_edges(true, false)
	if not source.begins_with("at"):
		log_error()
		return source
	source = source.trim_prefix("at")
	source = source.strip_edges(true, false)
	ret = Lexer.parse_identifier(source)
	source = ret[1]
	position = ret[0]
	source = source.strip_edges(true, false)
	if source.begins_with("with"):
		source = source.trim_prefix("with")
		source = source.strip_edges(true, false)
		ret = Lexer.parse_identifier(source)
		source = ret[1]
		animation = ret[0]
	return source
