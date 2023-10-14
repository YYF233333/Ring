class_name AST

var root: ProgramNode

func _init(source_code: String) -> void:
	root = ProgramNode.new()
	root.parse(source_code)

func codegen() -> String:
	return root.codegen()
