extends Node
## 游戏进度存储，最小不可分执行单元待脚本格式确定

# placehold
class Archive:
	var name: String
	var stage: String
	var script_execute_index: int # 当前执行到的脚本位置

var _archives_loaded: bool = false
var _archives: Dictionary # Dict[String, Archive]


## 服务器初始化
## param[in]: save_folder 存档文件夹路径
func init(save_folder: DirAccess) -> void:
	for archive_name in save_folder.get_files():
		var file_name := save_folder.get_current_dir() + "/" + archive_name
		var file := FileAccess.open(file_name, FileAccess.READ)
		if file != null:
			var archive := parse_archive(file.get_as_text())
			_archives[file_name] = archive

## 服务器析构
func deinit() -> void:
	_archives_loaded = false
	_archives = {}

static func parse_archive(content: String) -> Archive:
	return Archive.new()

