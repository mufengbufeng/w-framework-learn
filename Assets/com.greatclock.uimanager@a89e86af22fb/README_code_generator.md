# 初始界面逻辑代码生成工具

## 概述

本文档讲解了如何使用自定义的模板来创建[界面逻辑](README.md#界面逻辑)的初始代码。

设计并实现此部分解决方案，旨在简化[界面逻辑](README.md#界面逻辑)开发过程中的必要流程，降低学习成本。

## 关于模板

此模板的作用是，指导初始[界面逻辑](README.md#界面逻辑)的代码生成。

模板接收到的参数：界面绑定组件代码，即`MonoScript`对象。

需要向UI管理器中的逻辑代码生成工具提供以下内容：

- 生成代码的目标路径。
- 生成代码的命名空间。
- 生成的代码中[界面逻辑](README.md#界面逻辑)类的基类。
- 生成的代码中需要override的属性（property）列表。
- 生成的代码中`Open()`方法的定义方式。
- 生成的代码中`Close()`方法的定义方式。

此模板是编辑器代码中，实现了`IUIStackLogicTemplateDefine`和`IUIFixedLogicTemplateDefine`接口的类。

## 创建模板描述文件

模板可自动生成。在Project窗口的指定文件夹中，右键依次点击：`Create > UI Logic > Fixed Template Define`或`Create > UI Logic > Stack Template Define`，即可生成[固定层级界面](README.md#固定层级的界面)/[堆叠型界面](README.md#堆叠型的界面)的[界面逻辑](README.md#界面逻辑)代码模板。

你也可以在编辑器代码中自行创建实现了`IUIStackLogicTemplateDefine`或/和`IUIFixedLogicTemplateDefine`接口的类。

自动生成的模板可能不满足你实际项目的需求，此模板应根据实际项目中[界面逻辑](README.md#界面逻辑)基类而设计。

**注意**：如果工程中已经存在了继承自`IUIStackLogicTemplateDefine`或`IUIFixedLogicTemplateDefine`接口的类，则右键自动生成模板的选项为灰色不可点击状态。

## 创建界面逻辑初始代码

建议使用"自动生成"的方式来生成最初始的[界面逻辑](README.md#界面逻辑)代码，以保证其使用的基类正确，并实现必要的方法。其操作步骤如下：

1. 在Project窗口中找到界面prefab根节点上挂载的自动生成的绑定组件代码文件。
2. 选中该代码文件并右键点击，在弹出的菜单中依次点击：`Create > UI Logic > Stack UI Logic`或`Create > UI Logic > Fixed UI Logic`。

操作完成后，一个新的.cs文件将生成在模板描述中指定的路径下，其中包含了[界面逻辑](README.md#界面逻辑)的最基础代码。

**需要注意**：

- 生成代码的菜单仅在工程中配置了**用于自动生成[界面逻辑](README.md#界面逻辑)代码的描述模板**（模板）时才可使用，生成的代码的位置、格式、类名等信息，都在此模板中给出。
- 此操作仅在首次创建[界面逻辑](README.md#界面逻辑)的代码时使用，对于更新调整[界面逻辑](README.md#界面逻辑)中的任何内容，请手动编码。
