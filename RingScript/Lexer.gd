class_name Lexer

const WHITESPACE := [" ", "\t", "\n"]

static func parse_identifier(source: String) -> Array[String]:
	if source[0] in WHITESPACE:
		push_error("whitespace prefix during parsing identifier")
		return ["", source]
	for i in range(source.length()):
		if source[i] in [" ", "\t", "\n"]:
			var identifier := source.substr(0, i)
			var remain := source.substr(i)
			return [identifier, remain]
	# rare case, may be malicious input
	return [source, ""]
