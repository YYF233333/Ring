class_name ASTNode

var source: String ## source code related to the subtree

## Parser virtual method.[br]
## return value: [ matched , remain ]
func parse(source: String) -> Array[String]:
	push_error("Calling virtual parse method")
	return ["", source]

## Codegen virtual method.[br]
## return generated IR, just concat them.
func codegen() -> String:
	push_error("Calling virtual codegen method")
	return ""

## Print syntax error message to console.
## Override this method for detailed error message.
func log_error() -> void:
	print_rich("[color=red]Error occur in %s[/color]" % source)
