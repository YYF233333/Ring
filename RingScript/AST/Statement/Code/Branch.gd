class_name BranchNode
extends CodeStatementNode

var flag: ExpressionNode ## branch flag
var true_branch: Array[CodeStatementNode] ## if branch
var false_branch: Array[CodeStatementNode] ## else branch

func parse(source: String) -> String:
	if not source.begins_with("if"):
		return source
	source = source.trim_prefix("if")
	var ret := source.split(":", true, 1)
	flag = ExpressionNode.new()
	flag.parse(ret[0])
	source = ret[1]
	source = source.strip_edges(true, false)
