## 接口

```C++
class RingScript {
public:
    // 构造时传入source code编译为字节码
    RingScript(String source);
    // 返回字节码总数
    int get_total_inst_num();
    // 返回下一条要执行指令的编号
    int get_cur_inst_index();
    // 将下一条要执行指令设置为给定编号的指令
    void set_inst_index(int idx);
    // 执行下一条指令
    ErrorCode step(Node env);
};
```
