class_name ASTNode

## Parser virtual method.[br]
## Return unmatched part of source code.
func parse(source: String) -> String:
	push_error("Calling virtual parse method")
	return source

## Codegen virtual method, return generated IR.
func codegen() -> String:
	push_error("Calling virtual codegen method")
	return ""

## Print syntax error message to console.
## Override this method for detailed error message.
func log_error() -> void:
	print_rich("[color=red]Syntax Error![/color]")
