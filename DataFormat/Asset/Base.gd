class_name Asset
## Base Asset container with type included

var _data = null
var _type: AssetType

func _init(content, type: AssetType) -> void:
	_data = content
	_type = type

func is_type(type: AssetType) -> bool:
	return _type == type

func unwrap():
	push_error("Trying to unwrap a base Asset")
