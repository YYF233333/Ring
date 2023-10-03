class_name InstBlock

var insts: Array[Expression] = []

func _init(insts: Array[Expression] = []) -> void:
	self.insts = insts

func push(inst: Expression) -> void:
	insts.append(inst)
