# W-Framework

## 概述

此框架提供了"界面制作-界面逻辑-业务逻辑-网络请求"开发中全套工作流及相关工具。

设计、编写此框架，旨在：

- 方便项目快速构建完整且干净的基础工程。
- 实现统一业务开发流程，降低上手成本。
- 仅提供完整的上层逻辑，保证了框架对各种底层技术选型的适用性，方便移植复用。
- 简化网络请求的编码，简化逻辑中数据、事件的编码，简化界面逻辑对节点/组件的访问与操作，简化界面对数据变化的响应。
- 从框架及解决方案的底层设计上，避免编码中常见问题的产生。
- 第一时间暴露界面prefab中、访问界面节点时可能存在的问题，缩短开发测试周期。

此unity工程除了提供了完整的框架功能外，其本身就是个可交互的**功能说明**和**使用文档**。工程中所有名称中带有Demo的文件和文件夹，都是框架以外、起到文档作用的内容及相关资源。

### 框架定位

- 轻代码体量：除UniTask等广泛使用的代码库外，包括依赖的库在内仅有大约300k的运行时代码量。
- 纯C#代码方案：可接入hybridCLR等热更方案。
- 仅面向业务逻辑开发：提供了业务逻辑直接调用的代码，不干涉最底层的技术方案。
- 面向中重度游戏：大量系统玩法，大量网络请求，大量数据维护与同步。

### 部分核心的解决方案

- 异步RPC网络：一条语句即可请求服务器并获取到服务器的响应。
- 数据驱动：简洁优雅、一个小代码块就可以解决数据展示/数据计算随数据修改而同步的问题。
- UI管理器：固定层级/堆叠管理，界面自然分组，实体返回键响应及焦点界面，全屏界面，展示界面前异步准备等特性。
- UI绑定工具：低成本获取一切需要的节点/组件，彻底解决同时访问一节点上多个组件的问题，多级容器节点支持，模板节点实例管理等特性。
- 界面prefab检查工具：界面制作时第一时间发现各种潜在问题。

## 运行此工程

### 使用的Unity版本

使用2022.3.x进行开发。

此框架的核心全部为代码，可以向任何Unity版本迁移。

### 依赖库

- "unity_collections" from [github](https://github.com/greatclock/unity_collections) or [gitee](https://gitee.com/greatclock/unity_collections)：一些特殊用途的数据结构，目前只有用于排序的优先级队列。
- "data_driven" from [github](https://github.com/greatclock/data_driven) or [gitee](https://gitee.com/greatclock/data_driven.git)：数据驱动功能。
- "serialize_component_tool" from [github](https://github.com/greatclock/serialize_component_tool) or [gitee](https://gitee.com/greatclock/serialize_component_tool)：自动生成代码并绑定序列化节点/组件的编辑器工具。
- "unity_ui_manager" from [github](https://github.com/greatclock/unity_ui_manager) or [gitee](https://gitee.com/greatclock/unity_ui_manager)：UI管理器及prefab检查等编辑器工具。
- "unity_utils" from [github](https://github.com/greatclock/unity_utils) or [gitee](https://gitee.com/greatclock/unity_utils)：动画播放扩展、计时器等通用功能。
- [UniTask](TODO)：由Cysharp开发的专为Unity开发的轻量级异步编程库。

以上库都是通过git库方式被PackageManager导入，并定义在"Packages/packages-lock.json"文件中。

其中UniTask库位于github，可能需要手动使用其他导入方式。

### 启动

打开Assets/Scenes/DemoEntryScene.unity场景并运行。

**注意**：未接入任何资源方案，仅可在编辑器下运行。工程中所有代码和资源，在保证依赖和加载的前提下，可按需自由组织。

## 关于作者

电子邮件：thegreatclock@qq.com

WeChat：thegreatclock

QQ：368350561