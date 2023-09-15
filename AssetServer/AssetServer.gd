extends Node
## AssetServer Singeloton
## TODO: design asset index file format
## once we have index file, we could load it and have the whole
## metadata information about assets.


## Server initing state
var _index_loaded: bool = false
var _index: Dictionary

## 服务器初始化
## param[in]: index_file 索引文件
func init(index_file: FileAccess) -> void:
	var dict = JSON.parse_string(index_file.get_as_text())
	if dict is Dictionary:
		_index = dict
		_index_loaded = true

## 服务器析构
func deinit() -> void:
	_index_loaded = false
	_index = {}

func load_asset(name: String, type: AssetType) -> Asset:
	var ret: Asset = null
	match type:
		AssetType.IMAGE: ret = load_img(_index[name])
		AssetType.SCRIPT: ret = load_script(_index[name])
		AssetType.AUDIO: ret = load_audio(_index[name])
	return ret

static func load_img(path: String):
	pass

static func load_script(path: String) -> ScriptAsset:
	return ScriptAsset.new(FileAccess.get_file_as_string(path))
	
static func load_audio(path: String):
	pass
