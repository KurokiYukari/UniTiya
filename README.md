# UniTiya

UniTiya 是一个 Unity 的 3D ARPG 设计系统。它提供一个从零开始开发 ARPG 游戏的基础框架，这个框架涵盖了包括角色控制、视角控制、武器系统等 ARPG 的常见元素定义；并且对于各个元素的定义，都提供默认的可配置工具，通过序列化的方式配置其具体实现。通过 UniTiya，非程序专业的游戏策划、美术等人员可以以一种直观友好、不通过代码就能实现其各种各样的游戏设计想法；而程序人员也可以根据元素的定义进行自由地拓展，来实现更丰富的自定义功能。

手册： [UniTiya Manual](./Docs/UniTiya_Manual.md)

API 文档：[UniTiya API Doc 1.0](./Docs/API%20Doc/html/index.html)

## 使用准备

如果要拥有且要 `Dynamic Bone` 拓展，请在编译选项中添加 `DYNAMIC_BONE` 符号。
