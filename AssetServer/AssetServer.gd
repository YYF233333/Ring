extends Node
## AssetServer Singeloton
## TODO: design asset index file format
## once we have index file, we could load it and have the whole
## metadata information about assets.

static func load_img(path: String):
	## extension is part after last dot
	var ext := path.rsplit(".", true, 1)[0]
	if ext in ["png", "jpg", "svg"]:
		var img := load(path) as ImageTexture
		return img
	elif ext == "psd":
		push_error("psd format is not supported yet")
		return null
	else:
		push_error("Unable to load format %s" % ext)
		return null

static func load_script(path: String):
	return RingScript.compile(FileAccess.get_file_as_string(path))

static func load_audio(path: String):
	pass
