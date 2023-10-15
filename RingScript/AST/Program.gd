class_name ProgramNode
extends ASTNode

## AST root, just a bunch of statements

var statements: Array[StatementNode] = []

func parse(source: String) -> String:
	source = Lexer.skip_whitespace(source)
	while not source.is_empty():
		var statement := StatementNode.new()
		source = statement.parse(source)
		statements.append(statement)
		source = Lexer.skip_whitespace(source)
	return ""
