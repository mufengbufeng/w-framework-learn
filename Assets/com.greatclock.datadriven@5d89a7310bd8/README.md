# Data Driven

## Overview

The data-driven solution aims to solve the following problems:

* After data changes, the UI that displays the data is updated synchronously, with minimal content refresh.
* After data changes, the logic that depends on the data is re-executed, with the minimal possible scope of re-evaluation.

The data that changes can be a collection of fields, a number, or a piece of text.

This solution uses special data structures to implement a tree-like data structure with convenient member variable access.

Combined with the GreatEvent event system, this solution achieves data change events that are precise down to the leaf nodes of the data. At the same time, any change in a child node within a data node can be captured by the parent node's data change event.

Additionally, this data-driven solution supports array and dictionary data structures.

Recommended application scenarios for the data:

* **Logic State Management**: Data objects not only save the current state but can also directly delegate events when the state changes. For example, the open state of a limited-time event, or the state of a game phase.
* **Synchronization of Presentation and Data**: With a small amount of code, presentation elements can be associated with data. After the data changes, the corresponding display content can be modified, or other effects can be shown with minimal changes to the display elements.
* **Player Inventory Management**: When the quantity of items changes, the presentation can be quickly synchronized, and logic that depends on the item quantity can also be triggered quickly.

## Data Structure Definition

The data structure definition uses a combination of **manual writing** and **automatic generation**.

* **Manual Writing**: Used for the rapid definition of data structures.
* **Automatic Generation**: Enables the manually written data structure to support node events.

### Manual Data Structure Definition

First, create a new C# script and ensure that its filename is identical to the class name you are about to define (e.g., create a `PlayerData.cs` file for the `PlayerData` class). In this script, define a class that inherits from `RamDataCustomBase<T>`. In the class, declare `public` fields or properties as usual by using regular C# types (like `int`, `string`, `List<int>`).

**Example**: Creating `PlayerData.cs`

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

> **Important**:
>
> * **File Naming Rule**: The filename of the `.cs` file defining the data structure must be consistent with the name of a `public` class inside it.
> * **Access Permissions**: The data class itself must be `public`, and all fields or properties that need to be converted must also be `public`.
> * **Inheritance Rule**: The class must inherit from `RamDataCustomBase<T>`, and the generic parameter `T` must be the class itself.
> * **`partial` Keyword (Recommended)**: It is recommended to declare the data class as `public partial class`. The code generator will preserve the `partial` keyword, which allows you to add extra logic to the class in another file, providing great flexibility.

### Automatic Code Generation

**Note**: Before executing the code generation, please ensure that your project has no compilation errors to prevent unexpected results, such as the loss of modifications to the data structure.

In the Unity editor, click on the menu item `GreatClock > Data Driven > Regenerate Code` (Shortcut: Ctrl+Alt+Shift+D / Cmd+Option+Shift+D).

The framework will automatically scan for all qualifying classes and then rewrite the file, converting your handwritten simple fields (like `int`, `string`) into properties with full data-driven functionality (like `RamDataInt`, `RamDataString`).

The content of the generated `PlayerData.cs` file will look like this:

```csharp
using GreatClock.Framework;
using System.Collections.Generic;

public partial class PlayerData : RamDataCustomBase<PlayerData> {
    // Constructor, properties, events, etc., are auto-generated
    public PlayerData(IRamDataStructCtrl parent, out IRamDataCtrl ctrl) : base(parent, out ctrl) {
        // ...
    }
    public RamDataInt level { get; private set; }
    public RamDataString playerName { get; private set; }
    public RamDataList<RamDataInt> items { get; private set; }
    // ...
}
```

### Special Notes

* For common C# primitive types, `RamDataNodeValue.cs` has corresponding node types for data driving. After code regeneration, the primitive types will be converted to the new corresponding types.
* For list (`List<T>`)/array (`T[]`) type members, they will be converted to `RamDataList<T>` after code regeneration.
* For dictionary (`Dictionary<TKey,TVal>`) type members, they will be converted to `RamDataDict<TKey,TVal>` after code regeneration.

## Writing Data

After the data structure is generated, all member properties are read-only. You can only modify the values of the leaf nodes.

### Modifying Primitive Type Values

```csharp
// someStruct is an instantiated data object
someStruct.someStringNode.Value = "ABCD";
someStruct.someStruct.someIntNode.Value = 123;
```

### Modifying Lists

```csharp
// Add an element
var item1 = someStruct.someList.Add();
item1.someProp.Value = "abcd";

// Insert an element at a specific position
var item2 = someStruct.someList.Insert(0);
item2.someProp.Value = "efgh";

// Modify an existing element
var item3 = someStruct.someList[someStruct.someList.Count - 1];
item3.someProp.Value = "ijkl";

// Remove an element
someStruct.someList.RemoveAt(someStruct.someList.Count - 1);
```

### Modifying Dictionaries

```csharp
// Add a key-value pair
var val1 = someStruct.someDict.Add("key1");
val1.someProp.Value = "abcd";

// Modify an existing key-value pair
var val2 = someStruct.someDict["key2"];
val2.someProp.Value = "efgh";

// Safely modify
if (someStruct.someDict.TryGetValue("key3", out var val3)) {
    val3.someProp.Value = "ijkl";
}
```

### Syncing Lists from External Data

The `SyncFrom` extension method can be used to efficiently update a `RamDataList` with a regular collection.

```csharp
// someStruct.someList is a RamDataList<DataStructListItem>
// res.someArray is a List<SomeResArrayItem>

someStruct.someList.SyncFrom(res.someArray, (SomeResArrayItem from, DataStructListItem to) => {
    // 'from' is the source data, 'to' is the target object in the RamDataList
    // Write the conversion logic here
    to.prop1.Value = from.someProp1;
    to.prop2.Value = from.someProp2;
});

// Notify changes after sync
someStruct.CheckAndNotifyChanged();
```

### Batch Modifications and Notifications

For performance reasons, data modifications do not trigger notifications immediately. You need to manually call the `CheckAndNotifyChanged()` method on the root node after a series of modifications to trigger them all at once.

```csharp
// Make a series of modifications...
someStruct.someIntNode.Value++;
someStruct.someList.Add();

// Notify all changes at once
someStruct.CheckAndNotifyChanged();
```

* After this method is called, the framework will check for changes in the node and all its children and dispatch events for the changed nodes.
* Even if a node's value is restored to its original state after multiple modifications, an event will still be dispatched.

## Data Binding

There are three ways to respond to data changes, listed in order of recommendation.

### 1. Automatic Binding with `RamDataNodeBase.Watch()` (Recommended)

`Watch` is the core of the framework. It automatically tracks all data nodes accessed within an `Action` and re-executes the `Action` when any of these nodes change.

* When `Watch` is called, the callback is executed immediately once.
* During the execution of the callback, all accessed properties like `.Value` or collection properties like `.Count` are automatically "watched".
* When any of the "watched" nodes' data is changed and `CheckAndNotifyChanged()` is called, the callback will be automatically re-executed.

The `Watch` method returns an `IDisposable` object, which you need to save and call `.Dispose()` on when it's no longer needed to stop listening and avoid memory leaks.

**Complete Example**:

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
        // Create a data instance
        mPlayerData = new PlayerData(null, out _);

        // Use Watch to automatically update the UI
        mWatcher = RamDataNodeBase.Watch(() => {
            // In this Action, we access three data nodes:
            // mPlayerData.playerName, mPlayerData.level, mPlayerData.items
            // When any of them changes, this code will be re-executed automatically.
            nameText.text = mPlayerData.playerName.Value;
            levelText.text = "Lv: " + mPlayerData.level.Value;
            itemsText.text = "Items: " + mPlayerData.items.Count;
        });

        // Simulate data changes
        InvokeRepeating(nameof(ChangeData), 2f, 2f);
    }

    void OnDestroy() {
        // Stop watching when the object is destroyed
        mWatcher?.Dispose();
    }

    void ChangeData() {
        // Modify data values
        mPlayerData.level.Value++;
        mPlayerData.playerName.Value = "Player_" + UnityEngine.Random.Range(100, 999);
        mPlayerData.items.Add(); // Add a new item
        // Manual notification of changes is required
        mPlayerData.CheckAndNotifyChanged();
    }
}
```

> **Note**:
>
> * `Watch` only tracks data node read operations. If there are conditional branches in the callback, data nodes in the branches that are not executed will not be watched.
> * Do not use `.Bind()` inside a `Watch` callback, as this may cause more unrelated data nodes to be watched.

### 2. Using the `.Bind()` Extension Method (Recommended)

The `Bind` method provides a more traditional way of event binding. It directly connects the `onChanged` event of a data node to a callback function. Unlike `Watch`, `Bind` does not automatically track dependencies but precisely listens to the specific node you bind to.

The `Bind` method also returns an `IDisposable` object to help you manage its lifecycle.

**Value Type Binding Example**:

```csharp
// The callback function includes the new and previous values as parameters
IDisposable binding = someStruct.someIntNode.Bind((val, prev) => {
    Debug.Log($"Value changed from {prev} to {val}");
});

// When no longer needed
binding.Dispose();
```

**List Type Binding Example**:

```csharp
IDisposable listBinding = someStruct.someList.Bind((list, flags) => {
    // 'flags' can be used to determine if the list itself changed (add/remove)
    // or if the content of a child element changed.
    if (flags == eRamDataStructChangedType.Children) { return; }
    
    // Re-render the entire list...
});
```

### 3. Using Native `onChanged` Events (Not Recommended)

Each data node exposes an `onChanged` event. This is the lowest-level implementation, but it requires manual management of event registration and unregistration, which is error-prone and therefore not recommended for direct use.
