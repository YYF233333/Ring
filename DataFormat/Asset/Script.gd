class_name ScriptAsset
extends Asset

func _init(source_code: String) -> void:
	_data = source_code
	_type = AssetType.SCRIPT

func unwrap() -> RingScript:
	return RingScript.new(_data)
