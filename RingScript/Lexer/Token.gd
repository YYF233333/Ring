class_name Token

var type: String
var start: int ## start index of Token source
var len: int ## length of Token source

func _init(type: String, start: int, len: int) -> void:
	self.type = type
	self.start = start
	self.len = len

func source(full_source_code: String) -> String:
	return full_source_code.substr(start, len)
