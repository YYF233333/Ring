class_name BranchNode
extends CodeStatementNode

var flag: ExpressionNode ## branch flag
var true_branch: Array[CodeStatementNode] ## if branch
var false_branch: Array[CodeStatementNode] ## else branch
