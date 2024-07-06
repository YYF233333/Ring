---
_layout: landing
---

# This is the **HOMEPAGE**.

## Recent Updates

- 正在实现用户设置语句后是否继续执行的支持，目前在CodeBlock中先实现，将识别脚本语言的identifier设成false可以阻止自动继续执行，用于SubRuntime切换时防止脚本继续执行。

- 内置代码支持切换到了Python，解决了大量Nlua bug带来的问题，现在内置脚本可以和C#脚本一样访问引擎的所有功能，并且有良好的互操作性。

- 添加了存档支持，可以按Esc存档，存档位于snapshot文件夹下，开始游戏时按加载游戏可载入存档。
- 添加了Backlog支持，按Backspace回退一步，仅支持当前运行中看过的部分。