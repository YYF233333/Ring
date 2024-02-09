# RingScript设计文档

涉及模块：解释器、parser、IR

编译流程：source -> AST -> IR?

跳转语句是否能在AST中确定跳转点

~~解释器和编译器中哪一方需要了解对方实现~~

ban了变量和字面量之间的运算，因为要么需要编译期求值要么需要手动实现四则运算，不如直接用Expression类方便

## 语法

name: "content"

show character at left/middle/right (with ...)

hide character (with ...)

![](test.png)

<audio src=test.mp3></audio>

"# Chapter name"

label abababa:

```python
var a = 1
var b = "114"
var c = False
if c:
    goto "Chapter name"
else:
    interpreter_func_call(a, b)
```

## 语法格式

```
identifier; #非空字符串
string = [ { identifier | whitespace } ] ;
program = { statement | whitespace } ;
statement = say | show_character | hide_character | show_bg | play_audio | chapter_name | code_block ;
say = identifier, ": ", "\"",  string, "\"" ;
show_character = "show ", identifier, " at ", identifier, [ " with ", identifier ] ;
hide_character = "hide ", identifier, [ " with ", identifier ] ;
show_bg = "![", identifier, "](", identifier, ")" ;
play_audio = "<audio src=\"", identifier, "\"></audio>", " with ", identifier
chapter_name = "# ", string
code_block = "```", identifier, "\n", [ { code_statment | whitespace } ], "```"
code_statement = assignment | branch | goto
assignment = [ "var " ], identifier, " = ", Expression
branch = "if ", Expression, ":", { code_statment }, "else:", { code_statement }
goto = "goto ", identifier
Expression = function_call | VarRef | Arithmetic
function_call = identifier, "(", [ { Expression, "," } ], ")"
```

## 接口

```python
class RingScript:
    byte_code: list[ByteCode]
    def parse(source: str):
        pass

class ASTNode:
    name: str
    source: str
    children: list[ASTNode]
    
    def traverse(f: Callable):
        pass

class StatmentNode(ASTNode):
    pass
```

