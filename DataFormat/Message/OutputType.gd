class_name OutputType
## 输出消息类型

enum _Type {
	PrintToTextbox,
	ShowChapterName,
	HideUI,
	ShowUI,
}

static var PrintToTextbox := OutputType.new(_Type.PrintToTextbox)
static var ShowChapterName := OutputType.new(_Type.ShowChapterName)
static var HideUI := OutputType.new(_Type.HideUI)
static var ShowUI := OutputType.new(_Type.ShowUI)

var _type: _Type

func _init(type: _Type) -> void:
	_type = type
