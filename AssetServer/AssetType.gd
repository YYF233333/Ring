class_name AssetType

enum _Type {
	SCRIPT,
	IMAGE,
	AUDIO
}

static var SCRIPT := AssetType.new(_Type.SCRIPT)
static var IMAGE := AssetType.new(_Type.IMAGE)
static var AUDIO := AssetType.new(_Type.AUDIO)

var _type

func _init(type: _Type) -> void:
	_type = type
