class_name Lexer

var source: String
var index := 0 ## index of first character of next token

const WHITESPACE := [" ", "\t", "\n", "\r"]

const KEYWORDS = [
	"var", "if", "else", "goto", "show", "hide", "at", "with",
	"true", "false", "#"
]

func _init(source_code: String) -> void:
	source = source_code

func is_end() -> bool:
	return index >= source.length()

func peek() -> Token:
	pass

func next() -> Token:
	pass

## 当且仅当输入头部是 keyword + whitespace 时匹配
func match_keyword() -> Token:
	var input := source.substr(index)
	for keyword in KEYWORDS:
		if input.begins_with(keyword) and input[keyword.length()] in WHITESPACE:
			var token := Token.new(keyword, index, keyword.length())
			index += keyword.length()
			return token
		
	return null

## 输入头部与给定字符串相同时匹配
func match_custom(literal: String) -> Token:
	var input := source.substr(index)
	if input.begins_with(literal):
		var token := Token.new(literal, index, literal.length())
		index += literal.length()
		return token
	
	return null

## 匹配输入头部的前导空白字符
func match_whitespace() -> Token:
	var input := source.substr(index)
	var len := input.length()
	for i in range(input.length()):
		if input[i] not in WHITESPACE:
			len = i
	if len == 0:
		return null
	var token := Token.new("whitespace", index, len)
	index += len
	return token

## 匹配输入头部直到第一个空白字符为止的子串
func match_identifier() -> Token:
	var input := source.substr(index)
	var len := input.length()
	for i in range(input.length()):
		if input[i] in WHITESPACE:
			len = i
	if len == 0:
		return null
	var token := Token.new("identifier", index, len)
	index += len
	return token

## 匹配输入头部符合代码要求的变量名
func match_code_identifier() -> Token:
	var input := source.substr(index)
	var len := input.length()
	for i in range(input.length()):
		if input[i] in WHITESPACE:
			len = i
	if len == 0:
		return null
	var substr := input.substr(0, len)
	if not substr.is_valid_identifier():
		return null
	var token := Token.new("code_identifier", index, len)
	index += len
	return token
