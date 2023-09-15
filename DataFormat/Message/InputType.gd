class_name InputType
## 输入消息类型

enum _Type {
	ScriptStepForward,
	Save,
	Load,
}

static var ScriptStepForward := InputType.new(_Type.ScriptStepForward)
static var Save := InputType.new(_Type.Save)
static var Load := InputType.new(_Type.Load)

var _type: _Type

func _init(type: _Type) -> void:
	_type = type
