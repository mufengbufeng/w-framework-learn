# Data Driven

## 数据驱动概述

数据驱动方案旨在解决：

* 数据变更后，展示该数据的UI同步更新，并尽量保证最小的刷新内容；
* 数据变更后，依赖该数据的逻辑判断重新执行，并尽量保证最小的重新判断的范围。

变更的数据可能是包含了多个字段的数据集合，也可能是一个数字、一段文本。

此方案通过通过特殊的数据结构定义，实现了成员变量便携访问的树形数据结构。

此方案结合了GreatEvent事件系统，实现了数据变更事件精确到数据叶子节点。同时，一个数据节点中任意子节点有变动，此节点都可捕获到数据变更事件。

此外，此数据驱动方案还支持数组、字典数据结构。

数据的推荐应用场景：

* **逻辑状态管理**：数据对象在保存着当前状态的同时，还可以在状态变更时直接委派事件。 例如：限时活动开启状态，游戏阶段状等。
* **表现与数据的同步**：通过少量代码即可将表现元素与数据进行关联，在数据变更后，以最少显示元素的变更实现相应展示内容的修改或其他效果的展现。
* **玩家背包管理**：道具数量变更时可快速同步表现，依赖背包道具数量进行判断的逻辑也可快速被触发。

## 数据结构定义

数据结构定义使用 **手动编写** 和 **自动生成** 相结合的方式。其中：

* **手动编写**：用于实现数据结构的快速定义；
* **自动生成**：使手动编写的数据结构支持节点事件。

### 手动定义数据结构

首先，创建一个新的 C# 脚本，并确保其文件名与您将要定义的类名完全一致（例如，为 `PlayerData` 类创建 `PlayerData.cs` 文件）。在该脚本中，定义一个继承自 `RamDataCustomBase<T>` 的类。在类中，像平常一样声明 `public` 字段或属性，推荐使用普通的 C# 类型（如 `int`, `string`, `List<int>`）。

**示例**: 创建 `PlayerData.cs`

```csharp
using GreatClock.Framework;
using System.Collections.Generic;

public partial class PlayerData : RamDataCustomBase<PlayerData> {
    public PlayerData(IRamDataStructCtrl parent, out IRamDataCtrl ctrl) : base(parent, out ctrl) { }
    public int level;
    public string playerName;
    public List<int> items;
}
```

> **重要**:
>
> * **文件命名规则**：定义数据结构的 `.cs` 文件名，必须与内部的 `public` 类名保持一致。
> * **访问权限**：数据类本身必须是 `public`，其所有需要被转换的字段或属性也必须是 `public`。
> * **继承规则**：类必须继承自 `RamDataCustomBase<T>`，且泛型参数 `T` 是它自己。
> * **`partial` 关键字**：可以将数据类声明为 `public partial class`，此时代码生成器会保留 `partial` 关键字，这允许您在另一个文件中为该类添加额外的逻辑，非常灵活。

### 自动生成代码

**注意**：在执行生成代码前，请确保项目没有编译错误，以防止数据结构的修改丢失等意外结果。

在 Unity 编辑器中，点击菜单栏的 `GreatClock > Data Driven > Regenerate Code` (快捷键: Ctrl+Alt+Shift+D / Cmd+Option+Shift+D)。

框架会自动扫描所有符合条件的类，然后将该文件重写，把您手写的简单字段（如 `int`, `string`）转换为包含完整数据驱动功能的属性（如 `RamDataInt`, `RamDataString`）。

生成后的 `PlayerData.cs` 会包含自动生成的代码，看起来像这样:

```csharp
using GreatClock.Framework;
using System.Collections.Generic;

public partial class PlayerData : RamDataCustomBase<PlayerData> {
    // 构造函数、属性、事件等都会被自动生成
    public PlayerData(IRamDataStructCtrl parent, out IRamDataCtrl ctrl) : base(parent, out ctrl) {
        // ...
    }
    public RamDataInt level { get; private set; }
    public RamDataString playerName { get; private set; }
    public RamDataList<RamDataInt> items { get; private set; }
    // ...
}
```

### 特殊说明

* 对于常见的C#基础数据类型，`RamDataNodeValue.cs` 中都有相应的用于数据驱动的节点类型，在代码重新生成后，基础类型会变成对应的新类型。
* 对于列表(`List<T>`)/数组(`T[]`)类型的成员，在代码重新生成后，会变成`RamDataList<T>`类型。
* 对于字典(`Dictionary<TKey,TVal>`)类型的成员，在代码重新生成后，会变成`RamDataDict<TKey,TVal>`类型。

## 数据的写入

在数据结构生成后，所有成员属性均是只读的，您只能修改叶子节点中的值。

### 修改基础类型的值

```csharp
// someStruct 是一个已实例化的数据对象
someStruct.someStringNode.Value = "ABCD";
someStruct.someIntNode.Value = 123;
```

### 修改列表

```csharp
// 添加元素
var item1 = someStruct.someList.Add();
item1.someProp.Value = "abcd";

// 在指定位置插入元素
var item2 = someStruct.someList.Insert(0);
item2.someProp.Value = "efgh";

// 修改已有元素
var item3 = someStruct.someList[someStruct.someList.Count - 1];
item3.someProp.Value = "ijkl";

// 删除元素
someStruct.someList.RemoveAt(someStruct.someList.Count - 1);
```

### 修改字典

```csharp
// 添加键值对
var val1 = someStruct.someDict.Add("key1");
val1.someProp.Value = "abcd";

// 修改已有键值对
var val2 = someStruct.someDict["key2"];
val2.someProp.Value = "efgh";

// 安全地修改
if (someStruct.someDict.TryGetValue("key3", out var val3)) {
    val3.someProp.Value = "ijkl";
}
```

### 从外部数据同步列表

`SyncFrom` 扩展方法可以高效地用一个普通集合来更新 `RamDataList`。

```csharp
// someStruct.someList 是 RamDataList<DataStructListItem>
// res.someArray 是 List<SomeResArrayItem>

someStruct.someList.SyncFrom(res.someArray, (SomeResArrayItem from, DataStructListItem to) => {
    // 'from' 是源数据, 'to' 是 RamDataList 中的目标对象
    // 在这里编写转换逻辑
    to.prop1.Value = from.someProp1;
    to.prop2.Value = from.someProp2;
});

// 同步后通知变更
someStruct.CheckAndNotifyChanged();
```

### 批量修改与通知

为了性能，对数据的修改不会立刻触发通知。您需要在一系列修改完成后，手动调用根节点的 `CheckAndNotifyChanged()` 方法来统一触发。

```csharp
// 进行一系列修改...
someStruct.someIntNode.Value++;
someStruct.someList.Add();

// 一次性通知所有变更
someStruct.CheckAndNotifyChanged();
```

* 调用此方法后，框架会检查节点及其所有子节点的变动，并为有变动的节点派发事件。
* 即使一个节点的值在多次修改后恢复原状，事件依然会被派发。

## 数据驱动绑定

有三种方式可以响应数据的变化，推荐程度从高到低排列。

### 1. 使用 `RamDataNodeBase.Watch()` 自动绑定 (推荐)

`Watch` 是框架的精髓。它会自动追踪在一个 `Action` 中被访问过的所有数据节点，并在这些节点变化时自动重新执行该 `Action`。

* 调用 `Watch` 时，回调会立即执行一次。
* 在回调执行过程中，所有被访问的 `.Value` 或集合的 `.Count` 等属性都会被自动“监视”。
* 当任何被“监视”的节点数据发生变化并调用 `CheckAndNotifyChanged()` 后，回调会自动重新执行。

`Watch` 方法会返回一个 `IDisposable` 对象，您需要保存它并在不再需要时调用 `.Dispose()` 来停止监听，以避免内存泄漏。

**完整示例**:

```csharp
using UnityEngine;
using UnityEngine.UI;
using GreatClock.Framework;
using System;

public class PlayerView : MonoBehaviour {

    public Text nameText;
    public Text levelText;
    public Text itemsText;

    private PlayerData mPlayerData;
    private IDisposable mWatcher;

    void Start() {
        // 创建数据实例
        mPlayerData = new PlayerData(null, out _);

        // 使用 Watch 自动更新 UI
        mWatcher = RamDataNodeBase.Watch(() => {
            // 在这个 Action 中，我们访问了三个数据节点：
            // mPlayerData.playerName, mPlayerData.level, mPlayerData.items
            // 当它们中任何一个变化时，这段代码会自动重新执行。
            nameText.text = mPlayerData.playerName.Value;
            levelText.text = "Lv: " + mPlayerData.level.Value;
            itemsText.text = "Items: " + mPlayerData.items.Count;
        });

        // 模拟数据变化
        InvokeRepeating(nameof(ChangeData), 2f, 2f);
    }

    void OnDestroy() {
        // 4. 在对象销毁时，停止 Watch
        mWatcher?.Dispose();
    }

    void ChangeData() {
        // 修改数据值
        mPlayerData.level.Value++;
        mPlayerData.playerName.Value = "Player_" + UnityEngine.Random.Range(100, 999);
        mPlayerData.items.Add(); // 添加一个新物品
        // 需要手动通知变更
        mPlayerData.CheckAndNotifyChanged();
    }
}
```

> **注意**:
>
> * `Watch` 只关注数据节点的读取操作。如果回调中有条件分支，未执行到的分支中的数据节点将不会被监视。
> * 不要在 `Watch` 内部再使用 `.Bind()` ，这可能导致需要监视更多的无关数据节点。

### 2. 使用 `.Bind()` 扩展方法 (推荐)

`Bind` 方法提供了一种更传统的事件绑定方式，它将数据节点的 `onChanged` 事件直接连接到一个回调函数。与 `Watch` 不同，`Bind` 不会自动追踪依赖，而是精确地监听您所绑定的那一个节点。

`Bind` 方法同样会返回一个 `IDisposable` 对象，方便您管理生命周期。

**值类型绑定示例**:

```csharp
// 回调函数包含新值和旧值两个参数
IDisposable binding = someStruct.someIntNode.Bind((val, prev) => {
    Debug.Log($"Value changed from {prev} to {val}");
});

// 不再需要时
binding.Dispose();
```

**列表类型绑定示例**:

```csharp
IDisposable listBinding = someStruct.someList.Bind((list, flags) => {
    // flags 可以判断是列表自身变化 (增/删) 还是子元素内容变化
    if (flags == eRamDataStructChangedType.Children) { return; }
    
    // 重新渲染整个列表...
});
```

### 3. 使用原生 `onChanged` 事件 (不推荐)

每个数据节点都暴露了 `onChanged` 事件。这是最底层的实现，但需要手动管理事件的注册和注销，容易出错，因此不推荐直接使用。