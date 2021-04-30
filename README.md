# UniTiya

UniTiya 是一个 Unity 的基础游戏设计系统。它提供一个从零开始开发Unity 游戏的基础框架，这个框架涵盖了包括角色控制、视角控制、武器系统等 ARPG 的常见元素定义；并且对于各个元素的定义，都提供默认的可配置工具，通过序列化的方式配置其具体实现。通过 UniTiya，非程序专业的游戏策划、美术等人员可以以一种直观友好、不通过代码就能实现其各种各样的游戏设计想法；而程序人员也可以根据元素的定义进行自由地拓展，来实现更丰富的自定义功能。

手册： [UniTiya Manual](./Docs/UniTiya_Manual.md)

## 使用准备

+ Unity 版本要求 2020.3 或以上。
+ 依赖于 [InputSystem](https://docs.unity.cn/cn/2020.1/Manual/com.unity.inputsystem.html)，需要从 PackageManager 导入该包。
+ 依赖于 [UniRx](https://assetstore.unity.com/packages/tools/integration/unirx-reactive-extensions-for-unity-17276)，需要从 Asset Store 导入该资源。
+ 如果拥有且要使用 `Dynamic Bone` 拓展，请在编译选项中添加 `DYNAMIC_BONE` 符号。
