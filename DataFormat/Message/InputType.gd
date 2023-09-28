class_name InputType
## 输入消息类型

enum _Type {
	ScriptStepForward,
	SaveToFile,
	LoadFromFile,
	LoadFromHistory,
	UpdatePersistVariable,
}

static var ScriptStepForward := InputType.new(_Type.ScriptStepForward)
static var SaveToFile := InputType.new(_Type.SaveToFile)
static var LoadFromFile := InputType.new(_Type.LoadFromFile)
static var LoadFromHistory := InputType.new(_Type.LoadFromHistory)
static var UpdatePersistVariable := InputType.new(_Type.UpdatePersistVariable)

var _type: _Type

func _init(type: _Type) -> void:
	_type = type
