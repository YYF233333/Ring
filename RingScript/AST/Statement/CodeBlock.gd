class_name CodeBlockNode
extends StatementNode

var identifier: String ## language specified in markdown codeblock(unused)
var statements: Array[CodeStatementNode] ## statements in this block

func parse(source: String) -> String:
	if not source.begins_with("```"):
		return source
	source = source.trim_prefix("```")
	var ret := source.split("\n", true, 1)
	identifier = ret[0]
	identifier = identifier.strip_edges()
	ret = ret[1].split("```", true, 1)
	source = ret[1]
	var codes := ret[0]
	codes = codes.strip_edges()
	while not codes.is_empty():
		var statement := CodeStatementNode.new()
		codes = statement.parse(codes)
		statements.append(statement)
		codes = codes.strip_edges(true, false)
	return source
