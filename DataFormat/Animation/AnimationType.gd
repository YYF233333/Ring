class_name AnimationType

enum _Type {
	SCRIPT,
	IMAGE,
	AUDIO
}

static var SCRIPT := AnimationType.new(_Type.SCRIPT)
static var IMAGE := AnimationType.new(_Type.IMAGE)
static var AUDIO := AnimationType.new(_Type.AUDIO)

var _type

func _init(type: _Type) -> void:
	_type = type
