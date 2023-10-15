class_name AssignNode
extends CodeStatementNode

var identifier: String ## local variable name
var value: ExpressionNode ## value to be assigned

func parse(source: String) -> String:
	if not source.begins_with("var"):
		return source
	source = source.trim_prefix("var")
	var ret := source.split("=", true, 1)
	source = ret[1]
	identifier = ret[0]
	identifier.strip_edges()
	ret = source.split("\n", true, 1)
	source = ret[1]
	value = ExpressionNode.new()
	value.parse(ret[0])
	return source
