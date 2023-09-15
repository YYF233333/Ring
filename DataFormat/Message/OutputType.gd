class_name OutputType
## 输出消息类型

enum _Type {
	PrintText,
}

static var PrintText := OutputType.new(_Type.PrintText)

var _type: _Type

func _init(type: _Type) -> void:
	_type = type
