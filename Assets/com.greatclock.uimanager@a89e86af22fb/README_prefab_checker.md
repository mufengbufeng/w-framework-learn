# 界面prefab合理性检查工具

## 概述

界面管理器自身及项目实际情况都会对界面prefab资源制定相应的标准。

如果界面prefab资源未能满足全部标准，在其运行时就会出现相应的问题。这些问题通常有如下特性：

- 界面prefab资源修改到测试完成的周期长，涉及人员多，发现问题的耗时长。
- 资源变动成本远低于界面测试成本，无力人工排查并发现问题。
- 有些问题的出现条件可能比较苛刻，可能在调试、测试过程中都未被发现。
- 界面prefab的制作过程可能会有程序、策划、特效美术等多人先后参与，标准执行容易有遗漏，出错可能性大。

UI管理器中实现了界面prefab检查工具。

通过一小段编辑器代码即可为满足指定条件的prefab文件进行定制化的检查，以实现在资源制作的过程中，尽量检查并暴露出一些不符合制作标准的问题。

## 检查时机

- 打开prefab场景进行编辑时。
- 保存prefab时。

## 内置已实现的检查功能

UIPrefabBuildinCheckers.cs代码中，针对最常见的规则和标准实现了检查prefab的方法如下。

### 组件丢失脚本

方法名称：`CheckMissingReference()`。

用于检查并定位prefab中组件脚本文件丢失的情况。

###  直接或通过材质间接引用了图集中sprite对应的贴图

方法名称：`CheckReferencingTextureInAtlas()`。

用于检查并定位组件中引用了图集中sprite对应的texture资源的情况，此种引用会在运行时出现对贴图引用丢失的问题。

### Mask节点下不可被剪裁的内容

方法名称：`CheckContentInMask()`。

用于检查并定位Mask节点下不合理的Canvas或存在Renderer等不会被遮罩显示的情况。

### 界面中使用的sortingOrder范围是否越界

方法名称：`CheckCanvasSortingOrderRange()`。

用于统计界面中使用到的sortingOrder的最小和最大值，并检查其是否超过预定范围。因为UI管理器为每个界面分配的sortingOrder的范围是有限的。

### 界面根节点上的Canvas组件是否符合UI管理器的规范

方法名称：`CheckUIRootCanvas()`。

用于检查prefab的根节点上是否有正确可用的Canvas组件，以保证运行时对界面层级控制的正确。

### Canvas中有需要点击交互的组件但未挂GraphicRayCaster

方法名称：`CheckGraphicRaycasterOnCanvas()`。

用于检查并定位prefab存在需要点击交互的组件，但其所在的Canvas未挂有GraphicRaycaster组件，导致运行时无法点击交互的情况。

### AnimationClip非法控制部分属性

方法名称：`CheckIllegallyAnimated()`。

用于检查并定位prefab中的Animation/Animator组件的动画中，是否控制了需要被逻辑控制的属性。此情况会导致逻辑代码对该属性的控制失效。

## 关于检查prefab过程中的报错

报错的目的是：

- 保证prefab的制作者能够接收到明确的错误提醒。
- 让prefab制作者能够明确了解出错的原因。
- 让prefab制作者能够准确定位到出问题的节点和组件。

### 日志报错

- 报错等级：Warning、Error，分别对应`IPrefabCheckerError.LogWarning(string msg, Object context)`和`IPrefabCheckerError.LogError(string msg, Object context)`两个方法。
- 为方便界面编辑者从Console报错日志中快速定位出问题的节点，`context`参数应传入该条日志对应的出问题的节点`transform`对象用。
- 同一大类的问题，可进行多次日志报错。

### 对话框报错

- 对应方法：`IPrefabCheckerError.ShowDialog(string msg)`。
- 用于以最醒目的方式提示界面编辑者，该prefab出现了什么样的问题。
- 同一大类的问题，建议只弹出一次对话框。

## 为特定prefab指定检查规则

在工程的Editor文件夹中增加如下代码，即可为"Assets/Path/To/UIPrefabs"文件夹中的所有prefab进入指定的检查。检查prefab的方法应带有类型为`GameObject`和`IPrefabCheckerError`两个参数。

``` c#
public class UIPrefabChecker
{
    [PrefabChecker(false, "Assets/Path/To/UIPrefabs/")]
    static void TestChecker(GameObject prefab, IPrefabCheckerError err) {
        UIPrefabBuildinCheckers.CheckMissingReference(prefab, err);
        UIPrefabBuildinCheckers.CheckReferencingTextureInAtlas(prefab, err);
        UIPrefabBuildinCheckers.CheckContentInMask(prefab, err, false, true);
        UIPrefabBuildinCheckers.CheckCanvasSortingOrderRange(prefab, err, 100, true, false);
        UIPrefabBuildinCheckers.CheckUIRootCanvas(prefab, err);
        UIPrefabBuildinCheckers.CheckGraphicRaycasterOnCanvas(prefab, err);
        UIPrefabBuildinCheckers.CheckIllegallyAnimated(prefab, new ProtectedComponent[] {
            new ProtectedComponent() { node = prefab.transform, component = typeof(Transform) }
        }, err);
    }
}
```

其中`PrefabChecker`的定义为：

``` c#
public class PrefabCheckerAttribute : Attribute {
    public readonly bool IsRegex;
    public readonly string Pattern;
    public PrefabCheckerAttribute(bool isRegex, string pattern) {
        IsRegex = isRegex;
        Pattern = pattern;
    }
}
```

其中，`isRegex`为`true`时，工程中的prefab只要路径匹配正由表达式`pattern`，就会使用此检查方法进行检查；否则，若prefab的路径以`pattern`开头，则会使用此检查方法进行检查。
