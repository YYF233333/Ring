class_name ChapterNameNode
extends StatementNode

var chapter_name: String

func parse(source: String) -> String:
	var rule := RegEx.create_from_string(
		"[#]+[\\s]+(?<name>[\\s\\S]+)\\n"
		)
	var matched := rule.search(source)
	if matched == null or matched.get_start() != 0:
		return source
	chapter_name = matched.get_string("name")
	source = source.substr(matched.get_end())
	return source
