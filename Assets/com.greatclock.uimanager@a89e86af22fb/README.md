此代码库依赖：

- "unity_collections" from [github](https://github.com/greatclock/unity_collections) or [gitee](https://gitee.com/greatclock/unity_collections)。
- "unity_utils" from [github](https://github.com/greatclock/unity_utils) or [gitee](https://gitee.com/greatclock/unity_utils)。

# 概述

这是一个给Unity游戏使用的UI管理器。可以以 unity package 方式接入到工程中，此做法保证了UI管理器功能的独立，可同时服务于多个项目。

此外，此UI管理器在实现了众多基础功能的同时，还方便实现界面换皮、界面开启前异步准备、返回键响应及焦点管理等特性。

同时，在代码性能、高强度操作时的安全性上都有相应的处理。

## 名词解释及核心理念

### 界面逻辑

专指实现了`IUILogicStack`或`IUILogicFixed`的类（通常会继承自[指定的基类](#定制ui基类)），这些类主要有如下功能：

- 处理界面开启前的数据逻辑。
- 提供[用于界面展示的参数](#用于界面展示的参数)。
- 对[界面节点](#界面节点)进行操作，以使其展示相应的内容。
- 侦听用户对此展示界面可交互对应的操作，并做出相应响应。
- 响应界面的生命周期事件，并进行相应的逻辑处理。

界面逻辑的类型由[用于界面开启的参数](#用于界面开启的参数)指定，界面逻辑的实例由UI管理器来创建和管理。

### 界面节点

专指在运行时通过[IUILoader对象](#界面开启参数获取及gameobject加载器)获取的界面prefab资源的实例，通常包括：

- 控制渲染层级等规则的Canvas组件。
- 界面中需要展示贴图、精灵、文本等内容，以及交互组件。
- 界面中内嵌的3D模型、粒子物资等其他Renderer渲染的内容。
- `Animation`及`Animator`等动画组件。

### 界面层级

即界面与界面之前的显示（渲染）先后顺序，使用`Canvas`及`Renderer`组件的`sortingOrder`属性来控制。

考虑到UI组件中可能会穿插渲染由UI摄像机拍照的3D物体（不透明物体），这些物体会优先于所有UI渲染，渲染UI内容时需要判断此UI要显示在此3D物体之前或者之后。所以所有UI界面及其中的内容要充分利用深度（体现在Z轴位置上）。

由于`sortingOrder`值更大的内容会显示在上层，其Z轴（相对于摄像机坐标系）位置值越小的内容也会显示在最上层，所以在此UI管理器中，将二者绑定，其规则见[层级管理规则，Z轴管理规则](#层级管理规则，z轴管理规则)。

### 全屏界面

此概念仅适用于[堆叠型的界面](#堆叠型的界面)，并作为[堆叠型的界面](#堆叠型的界面)的一个属性。

全屏界面通常使用如下两种显示方式中的一种：

- 界面中显示内容覆盖整个屏幕，不会显示出此界面下面的界面及场景。
- 界面有镂空，需要显示出下面的场景，但不显示此前已打开的[堆叠型的界面](#堆叠型的界面)。

若显示了全屏界面，则此前已打开的其他[堆叠型的界面](#堆叠型的界面)将被[隐藏](#关于界面隐藏)。

### 界面自然分组

在游戏开发过程中，可能会有如下界面的开启关系：

- 用户在**限时商城界面**中点击一个商品，打开**商品信息界面**。
- 接着，此用户在**商品信息界面**点击了购买按钮，弹出了游戏内的**支付确认界面**。

如果玩家在进行上述操作的时间，恰好处于**限时商城界面**开启的最后几秒，并停留在**支付确认界面**没有操作。**限时商城**到期后，需要关闭**限时商城界面**，同时也要附带关闭所有随之打开的**商品信息界面**以及**支付确认界面**。

针对上面的情况，可以将此三个界面安排在同一个属于**限时商城界面**的自然分组中，在关闭界面时，调用[按照自然分组关闭界面](#是否关闭自然组中的后续界面)的API关闭**限时商城界面**时，即可关闭上述三个界面。

在上述案例中，若限时商城未到达关闭时间，但用户打开的是个**限时商品**的**商品信息界面**，随后打开对应的**支付确认界面**并停留。稍后此**限时商品**到期，需要关闭**商品信息界面**。此时虽然三界面同属**限时商城界面**的自然分组，但关闭**商品信息界面**时，仅会关闭此界面及后续打开的**支付确认界面**。

### 界面分类

#### 堆叠型的界面

这一类型的界面使用"**栈**"的方式来管理，通常具备以下特点：

- 最新打开的界面显示在最上方。
- 最上方的界面关闭后，次上方的界面将显示在最上方。
- 通常是最上方的界面响应用户的操作。

堆叠型界面的[界面逻辑](#界面逻辑)应实现`IUILogicStack`接口。

#### 固定层级的界面

这一类型的界面的`sortingOrder`固定，所在的Z轴位置也固定。这两个参数的计算规则详见[层级管理规则，Z轴管理规则](#层级管理规则，z轴管理规则)，其参数来自`IUILogicFixed.SortingOrderBias`，详见[用于固定层级界面的展示参数](#用于固定层级界面的展示参数)。

固定层级界面的[界面逻辑](#界面逻辑)应实现`IUILogicFixed`接口。


### 焦点界面

焦点界面有两层含义：

- 当前处在最前面的界面，即用户可直接交互的界面。
- 需要响应实体返回键（开发中的ESC键）按下事件的界面。

[堆叠型的界面](#堆叠型的界面)，都是可以获得焦点的界面。

某特定时刻的焦点界面，通常是最上方的[堆叠型的界面](#堆叠型的界面)。但层级更高的[固定层级界面](#固定层级的界面)，也可临时获取焦点，以避免所有低层级的界面获取焦点。

### 全局唯一的UI根节点

其中，**全局唯一**指的是：

- 仅有一个UI根节点。
- 从程序启动开始，始终使用一个UI根节点。

这个[UI根节点](#创建场景中ui根节点)的作用是：

- 为UI管理器提供[界面节点](#界面节点)的父节点。
- 通过其中的Canvas及相关组件配置渲染方式及屏幕适配方案。
- 通过`UIRoot`组件来提供[层级计算](#层级管理规则，z轴管理规则)参数等全局配置。

### 不干涉UI管理器使用到的外部技术方案

此UI管理器中可能需要使用到的外部支持：

- prefab资源实例的异步获取与回收。
- 通过界面id获取[用于界面开启的参数](#用于界面开启的参数)过程中，可能会用到本地配置方案。
- 界面节点、组件绑定方案。
- 界面组件代码、[界面逻辑](#界面逻辑)代码的热更新方案。

其中，前两项需要在调用UI管理器的初始化方法传入的[参数](#界面开启参数获取及gameobject加载器)对象中实现，后两项可根据实际项目情况及个人喜好，进行方案选择。

### 不干涉界面逻辑类的继承关系

[界面逻辑](#界面逻辑)应实现该界面[分类](#界面分类)所对应的接口，UI管理器通过这些接口中的属性和方法获取[用于界面展示的参数](#用于界面展示的参数)以及调用[界面生命周期](#界面生命周期)方法。

所以[界面逻辑](#界面逻辑)可继承自任何实现了项目中所需属性和方法的基类，以使界面逻辑中方便调用项目中定制的方法。

但需要注意：[界面逻辑](#界面逻辑)类应包含`public`无参构造函数。

### 加载界面节点的同时可以有其他异步操作

对于游戏开发中的如下常见需求，可直接在对应的[界面逻辑](#界面逻辑)中编码实现：

- 打开排行榜界面时，需要在界面展示前需要完成排行榜数据的获取。
- 在显示某个界面前，需要切换到指定场景。

此方案允许开发者在加载[界面节点](#界面节点)（加载界面prefab资源）的时候同时进行[界面逻辑](#界面逻辑)需要的其他异步操作，并可控制此异步操作的超时时长及超时行为，并可以在异步操作失败后关闭界面。

相比于在**展示界面之前**或**展示界面之后**再进行这些异步操作，在加载界面过程中进行这些异步操作的意义在于：

- 统一了打开界面的写法：不需要在调用`UIManager.Open`方法之前进行任何异步的准备（除参数外），避免了为打开某些界面而进行的特殊处理的编码。
- 避免了因没有准备好数据、场景等内容而显示异常的尴尬：界面在[界面节点](#界面节点)加载完成，且异步准备成功完成后才会显示，而在加载、异步准备过程中，有[屏蔽用户操作的机制](#ui加载过程中屏蔽用户操作机制)来保证当前正在进行的操作的完整与安全。

# 规则详解

## 层级管理规则，Z轴管理规则

在此方案中，界面的Z轴位置取决于界面基础`sortingOrder`的值。

所有界面的`sortingOrder`的基础值，都是动态计算获得并在界面启动时动态赋值。

计算界面`sortingOrder`值及Z轴位置所依赖的常数有：

- 最小/最大`sortingOrder`值；
- 每个UI界面占用`sortingOrder`的范围；
- 相邻两界面之间的Z轴位置间隔。

这些常数，都以组件参数的形式定义在`UIRoot`组件中，结合场景中的UI相机等节点组件的参数，可得到计算界面`sortingOrder`值及Z轴位置所依赖的其他参数：

- 最低层级的界面与最高层级界面所对应的最远与最近的Z轴位置（低层级界面渲染优先级低，距离远）；

计算`sortingOrder`的规则：

- 对于堆叠界面：最小`sortingOrder`值 + (2 + 界面堆叠索引)  \* 每个UI界面占用`sortingOrder`的范围；
- 对于固定层级界面：[界面逻辑](#界面逻辑)的`SortingOrderBias`为负数时，其`sortingOrder`值为最大`sortingOrder`值与该负值之和；为正数时，其`sortingOrder`值为最小`sortingOrder`值与该正值之和。

计算Z轴位置的规则。

- `sortingOrder`值越大的，Z轴位置值越小；
- 对于堆叠界面和`SortingOrderBias`值为正数的固定层级界面，其Z轴位置值基于最远的Z轴位置来计算；
- 对于`SortingOrderBias`值为负数的固定层级界面，其Z轴位置值基于最近的Z轴位置来计算。

## 关于UI管理器需要的界面参数

### 用于界面开启的参数

此UI管理器认为：

- 一个**界面**由"用于展示的gameObject"和对应的"控制逻辑"构成。
- `id`用于一个界面的唯一标识。

所以，UI管理器需要通过下面的参数来确定一个**界面**：

- 界面prefab路径，对应下面代码中`prefab_path`字段。
- 界面控制逻辑的类型，对应下面代码中`logic_type`字段。

UI管理器中定义了`ParametersForUI`类型，作为开启界面时的必需参数：

``` c#
public struct ParametersForUI {
    public string id;
    public string prefab_path;
    public Type logic_type;
}
```

关于开启界面时这些参数使用，详见：

- [开启界面的API](#开启界面的api)中[指定id或提供全部所需要的参数](#指定id或提供全部所需要的参数)。
- [界面开启参数获取及GameObject加载器](#界面开启参数获取及gameobject加载器)中关于`GetParameterForUI`的部分。

### 用于界面展示的参数

这部分参数均从[界面逻辑](#界面逻辑)的实例中获取，这些参数均定义在[界面逻辑](#界面逻辑)需要实现的接口中。
#### 通用的界面展示参数

``` c#
// 此接口为IUILogicStack与IUILogicFixed的父接口。
public interface IUILogicBase {
    string MutexGroup { get; }
    eUIVisibleOperateType VisibleOperateType { get; }
    ...
}
```


其中：

- `MutexGroup`：互斥组。对于互斥组相同的两个界面，在打开新的界面时，旧界面将被关闭。
- `VisibleOperateType`：指定该界面在需要被隐藏时，通过何种方式（可多选）来隐藏。参考[关于界面隐藏](#关于界面隐藏)中[如何隐藏](#如何隐藏)。

#### 用于堆叠型界面的展示参数

``` c#
public interface IUILogicStack : IUILogicBase {
    // 此界面是否允许多开。
    bool AllowMultiple { get; }
    // 此界面是否为全屏界面。
    bool IsFullScreen { get; }
    // 是否开户新的自然分组。
    bool NewGroup { get; }
}
```

其中：

- `AllowMultiple`：先后开启同一个id的界面时（其中可以有其他界面），如果此界面允许多开，则正常显示新开户的同id界面，此时这个id的界面存在多个实例；如果此界面不允许多开，则会关闭之前开户的同id的界面。
- `IsFullScreen`：标记此界面是否为[全屏界面](#全屏界面)。
- `NewGroup`：是否从此界面开始，创建新的[自然分组](#界面自然分组)。如果是，则此界面将不再属性之前界面的[自然分组](#界面自然分组)，此后开启的界面也会默认属于此界面相同的[自然分组](#界面自然分组)。

#### 用于固定层级界面的展示参数

``` c#
public interface IUILogicFixed : IUILogicBase {
    int SortingOrderBias { get; }
}
```

其中：

- `SortingOrderBias`：用于指定此界面的[层级](#界面层级)：此值为负数时表示对于最高层级对应`sortingOrder`的偏移，即从最大的`sortingOrder`值往下减；此值为0或正数时表示对最低的层级对应`sortingOrder`的偏移，即从最小的`sortingOrder`值往上加。详见[层级管理规则，Z轴管理规则](#层级管理规则，z轴管理规则)。

## 界面开启流程

1. 获取[界面开启参数](#用于界面开启的参数)（如果在使用界面id开启界面），参数无效则放弃开启界面。
2. 获取[界面逻辑](#界面逻辑)实例。
3. 调用[界面逻辑](#界面逻辑)实例的`OnCreate()`方法，并传入逻辑给出的参数，如果方法返回值为`false`则放弃开启界面。
4. 开启LoadingOverlay以避免屏蔽用户的其他操作。
5. 同时开始[界面节点](#界面节点)的加载与尝试调用界面开启前的[异步准备方法](#加载界面节点的同时可以有其他异步操作)：
	1. 调用[界面逻辑](#界面逻辑)的`OnPrepareCheck()`方法，如果方法返回`false`，则不再调用`OnPrepareExecute()`方法。
	2. 调用`OnPrepareExecute()`方法（如果需要）。
6. 等待[界面节点](#界面节点)加载完成与`OnPrepareExecute()`方法（如果需要）执行完成或超时。若`OnPrepareExecute()`方法异步返回了`false`或设置了超时需要关闭界面，则放弃开启界面。
7. 设置界面中`Canvas`与`Renderer`的`sortingOrder`。
8. 调用界面的`OnOpen()`和`OnShow()`方法。
9. 处理其他界面的[隐藏](#关于界面隐藏)、互斥等关系。
10. 关闭LoadingOverlay以使用户可以进行交互。

详解：

- [界面逻辑](#界面逻辑)在开启界面的流程中同步创建，`OnCreate()`方法也被同步调用。此设计在一定程度上让[界面逻辑](#界面逻辑)中的部分逻辑脱离[界面节点](#界面节点)，并在[界面节点](#界面节点)加载前就开始执行。
- 在调用`OnCreate()`方法、`OnPrepareExecute()`方法过程中，[界面逻辑](#界面逻辑)均有终止界面开启流程的可能。
- 在`OnOpen()`方法中，[界面节点](#界面节点)`gameObject`对象将被传入到[界面逻辑](#界面逻辑)实例中，带有表现的[界面逻辑](#界面逻辑)正式启动。

## 界面生命周期

生命周期方法均成对出现，且必定成对被调用。

由于[界面逻辑](#界面逻辑)在未开始加载[界面节点](#界面节点)时就已经开始运行，所以界面生命周期方法中，将包含**纯逻辑**的生命周期方法，以及**界面显示**相关的生命周期方法。

### 生命周期方法

| 方法名称           | 触发时机               | 可重复 | 参数                                 |
| -------------- | ------------------ | --- | ---------------------------------- |
| `OnCreate`     | [界面逻辑](#界面逻辑)启动时   | 否   | 打开界面时传入的入口参数                       |
| -`OnOpen`      | [界面节点](#界面节点)加载完成时 | 否   | 界面`gameObject`对象及基础`sortingOrder`值 |
| --`OnShow`     | [界面节点](#界面节点)显示时   | 是   | 是否是首次调用                            |
| --`OnHide`     | [界面节点](#界面节点)隐藏时   | 是   | 无                                  |
| -`OnClose`     | [界面节点](#界面节点)关闭回收前 | 否   | 无                                  |
| `OnTerminated` | [界面逻辑](#界面逻辑)结束时   | 否   | 无                                  |

其中：

- 方法名称前的"-"表示该生命周期方法的深度，更深层的生命周期方法必会在浅层的生命周期那一对方法的两次调用之中进行。
- "可重复"表示在一次界面启动到关闭的生命周期中，一个生命周期方法是否可以被触发多次。
- 通常`OnShow()`方法的首次调用会紧随`OnOpen()`之后，除非在此界面未成功展示的时候，有一个加载更快的[全屏界面](#全屏界面)完成加载并已经显示。
- 对于一个正在显示的界面被关闭时，其`OnHide()`、`OnClose()`和`OnTerminated()`会被连续调用。

注意：

- 仅`OnCreate()`方法返回`true`的时候才可能会调用后续的生命周期方法，包括与之对应的`OnTerminated()`。

### 侦听被开启界面的生命周期

此操作需要在调用[开启界面的API](#开启界面的api)中的`OpenWithHandler()`方法中传入[用于侦听目标界面生命周期事件的对象](#用于侦听目标界面生命周期事件的对象)，此对象应实现`IUIEventHandler`接口：

``` c#
public interface IUIEventHandler {
    void OnOpened();
    void OnShown();
    void OnHided();
    void OnClosed();
    void OnTerminated();
}
```

其中：

- 方法名称与[界面逻辑](#界面逻辑)中的相应方法有所不同，因为[用于侦听目标界面生命周期事件的对象](#用于侦听目标界面生命周期事件的对象)中的生命周期方法是在调用了[界面逻辑](#界面逻辑)中的相应方法之后才调用。
- 这些方法中并没有与`OnCreate()`对应的方法，因为在调用`UIManager.OpenWithHandler()`方法时，[界面逻辑](#界面逻辑)的`OnCreate()`就已经被调用，且`UIManager.OpenWithHandler()`方法的返回值也表明了该界面是否会继续进行加载等后续[流程](#界面开启流程)。

## 开启界面的API

开启界面的方法，需要以下三种参数：

- 用于指定目标界面的id或[界面开启参数](#用于界面开启的参数)（必需）；
- [界面逻辑](#界面逻辑)启动时需要的入口参数（可选）；
- [用于侦听目标界面生命周期事件的对象](#用于侦听目标界面生命周期事件的对象)（可选）。

注：对于以上参数的所有组合，在UI管理器中都对应一个开启界面的方法。

### 用于指定目标界面的id或界面开启参数

在UI管理器的开启界面的一系列方法中，使用了两种方式来确定一个界面：

- 使用字符串id：方便业务逻辑直接开启界面，无需关心其开启参数。但需要调用[UI管理器初始化](#运行时初始化ui管理器需要的外部功能)时传入的[界面开启参数获取及GameObject加载器](#界面开启参数获取及gameobject加载器)中`GetParameterForUI`方法以获取全部参数。
- 使用`ParametersForUI`数据结构提供全部界面开启参数：方便项目定制。

典型方法如下：

``` c#
public static class UIManager {
    ...
    public static bool Open(string id) { }
    public static bool Open(ParametersForUI cfg) { }
    ...
}
```

### 界面逻辑启动时需要的入口参数

对于UI管理器的开启界面的部分方法，允许向[界面逻辑](#界面逻辑)传入一个类型为`object`的参数（详见[界面开启流程](#界面开启流程)中关于`OnCreate`的部分）。

相关开启界面的方法如下：

``` c#
public static class UIManager {
    ...
    public static bool Open(string id, object parameter) { }
    public static bool Open(ParametersForUI cfg, object parameter) { }
    public static bool OpenWithHandler(IUIEventHandler handler, string id, object parameter) { }
    public static bool OpenWithHandler(IUIEventHandler handler, ParametersForUI cfg, object parameter) { }
    ...
}
```

### 用于侦听目标界面生命周期事件的对象

UI管理器开启界面的方法中，方法名为`OpenWithHandler`的方法，需要传入一个[侦听界面生命周期事件](#侦听被开启界面的生命周期)的对象，从而实现开启界面的逻辑中响应被开启界面的各个生命周期事件，并执行相应的逻辑。

此逻类事件侦听辑通常用于：

- 连续开启多个界面时，侦听前一个界面的关闭事件。
- 在进行较大的界面加载时，先完成“加载进度界面”的展示，之后再进行较大界面的加载，此时需要侦听较大界面的展示事件。

相关开启界面的方法如下：

``` c#
public static class UIManager {
    ...
    public static bool OpenWithHandler(IUIEventHandler handler, string id) { }
    public static bool OpenWithHandler(IUIEventHandler handler, string id, object parameter) { }
    public static bool OpenWithHandler(IUIEventHandler handler, ParametersForUI cfg) { }
    public static bool OpenWithHandler(IUIEventHandler handler, ParametersForUI cfg, object parameter) { }
    ...
}
```

## 关闭界面的API

### 如何指定要关闭的界面

#### 按界面id指定

需要注意：

- 界面id用于界面**配置**层面的唯一标识，并非界面实例的唯一标识。
- 对于可多开界面（见[用于界面展示的参数](#用于界面展示的参数)中的"允许多开"），无法通过界面id来确定出唯一的界面实例。
- 按界面id关闭界面时，会关闭所有id匹配的界面。

相关方法：

``` c#
public static class UIManager {
    ...
    public bool CloseGroup(string id) { }
    public bool CloseSingle(string id) { }
    ...
}
```

#### 按界面逻辑实例指定

不同于按界面id指定，使用[界面逻辑](#界面逻辑)实例可以绝对唯一指定到目标界面。

相关方法：

``` c#
public static class UIManager {
    ...
    public bool CloseGroup(IUILogicBase logic) { }
    public bool CloseSingle(IUILogicBase logic) { }
    ...
}
```

关于用于[界面逻辑](#界面逻辑)实例`IUILogicBase`，参见[界面分类](#界面分类)以及[定制UI基类](#定制ui基类)。

### 是否关闭自然组中的后续界面

调用名称为`CloseGroup()`的关闭界面方法时，将会关闭目标界面以及后续界面同与目标界面属于同一自然分组的界面。

调用名称为`CloseSingle()`的关闭界面方法时，不会考虑界面的自然分组，仅会关闭目标界面。

## 关于界面隐藏

### 何时隐藏

显示[全屏界面](#全屏界面)时，其下已显示的[堆叠界面](#堆叠型的界面)需要隐藏。

### 如何隐藏

由[界面逻辑](#界面逻辑)的[用于界面展示的参数](#用于界面展示的参数)中`VisibleOperateType`属性来控制其隐藏方法。

`VisibleOperateType`属性类型`eUIVisibleOperateType`的定义为：

``` c#
[Flags]
public enum eUIVisibleOperateType {
    SetActive = 0,
    LayerMask = 1,
    OutOfScreen = 2
}
```

其中：

- `SetActive`：在未选择其他的任何一种方式时采用的控制显隐默认方式，如果采用了其他方式中的任何一种，都不会再使用此方式来控制显示与隐藏。
- `LayerMask`：使用`Canvas`及`Renderer`的`Layer`属性来控制显示与隐藏。在将其隐藏时，将会使用到[UI根节点](#全局唯一的ui根节点)中的`Layer For Hide`参数，详见[创建场景中UI根节点](#创建场景中ui根节点)。
- `OutOfScreen`：使用移出屏幕显示范围的方法来隐藏界面。

引入多种隐藏界面的方法，是为了避免使用单一的SetActive方式。因为SetActive方式会造成非常多节点组件的生命周期事件调用，占用CPU时间。

## 焦点界面管理

焦点界面永远是需要所有需要焦点的界面中，层级（`sortingOrder`）最高的，所以，通常是最上方的[堆叠型的界面](#堆叠型的界面)。

### 堆叠界面的焦点

- 通常是最上方的界面获取焦点。
- [界面逻辑](#界面逻辑)启动后即可获取焦点，所以应注意[界面逻辑](#界面逻辑)的`OnESC()`方法执行时，可能界面还未显示。

### 固定层级界面临时获取焦点

[固定层级界面](#固定层级的界面)的[界面逻辑](#界面逻辑)也可通过实现`IUIDynamicFocusable`接口，在需要的时候临时获取焦点。

相关接口的定义为：

``` c#
public interface IUIDynamicFocusable : IUIFocusable {
    void SetDynamicFocusAgent(IUILogicDynamicFocusAgent agent);
}

public interface IUILogicDynamicFocusAgent {
    bool RequireFocus();
    bool ReleaseFocus();
}
```

在实现了`IUIDynamicFocusable`接口的[固定层级界面](#固定层级的界面)的[界面逻辑](#界面逻辑)启动后，UI管理器会传给它一个实现了`IUILogicDynamicFocusAgent`接口的对象。在此[界面逻辑](#界面逻辑)需要焦点或释放焦点时，可以调用此对象的`RequireFocus()`和`ReleaseFocus()`方法是临时申请获取和释放焦点。

申请获取焦点后，该[固定层级界面](#固定层级的界面)也要与其他需要焦点的界面进行层级（`sortingOrder`）比较，只有最高层级的需要焦点的界面，才能最终获取焦点。

# 项目接入

## 创建场景中UI根节点

场景中UI根节点的核心：`UIRoot`组件。其包含了如下参数：

- `Root Canvas`：指定了UI渲染方案、屏幕适配的根`Canvas`；
- `Parent For UI`：所有UI[界面节点](#界面节点)将使用的父节点；
- `Layer For Hide`：用于使用Layer[隐藏界面](#如何隐藏)时设置的不可见的Layer；
- `Sorting Order Min`、`Sorting Order Max`、`SortingOrder Range Per UI`、`Position Z Interval`：用于、层级和Z轴位置的计算参数；
- `Off Screen Position Delta`：移出屏幕范围的[隐藏方式](#如何隐藏)中，使用的位置偏移数值。

## 运行时初始化UI管理器需要的外部功能

调用`UIManager.(IUILoader uiLoader, IUILoadingOverlay loadingOverlay, bool useLogicCache)`方法来初始化其所需要的外部功能。

### 界面开启参数获取及GameObject加载器

UI管理器中定义了`IUILoader`接口，项目需要实现其中的方法。

``` c#
public interface IUILoader {
    ParametersForUI GetParameterForUI(string id);
    UniTask<GameObject> LoadUIObject(string path);
    void UnloadUIObject(GameObject go);
}
```

其中：

- `GetParameterForUI`用于以id指定界面方式[开启界面](#开启界面的api)时，获取全部[界面开启参数](#用于界面开启的参数)。
- `LoadUIObject`和`UnloadUIObject`用于加载和卸载prefab的方法。

### UI加载过程中屏蔽用户操作机制

`IUILoadingOverlay`接口定义：

``` csharp
public interface IUILoadingOverlay {
    void BeginLoading(string key);
    void EndLoading(string key);
}
```

### 是否使用界面逻辑实例缓存

[界面逻辑](#界面逻辑)的实例可以重复使用，以减少`GC Alloc`。使用[界面逻辑](#界面逻辑)实例缓存时，应注意：

- [界面逻辑](#界面逻辑)应在各[生命周期](#界面生命周期)结束的方法中，清理相应成员变量、注册侦听及各种状态。
- 将项目的[界面逻辑](#界面逻辑)实例改为使用缓存后，可能因已存在[界面逻辑](#界面逻辑)代码中存在上述问题而导致在第二次或以后开启同个界面时，存在各种不可预知的错误。
- 对内存和GC不敏感的项目，建议不要开启，以降低界面逻辑出错的可能。

## 定制UI基类

UI管理器对UI[逻辑](#界面逻辑)类的要求为：

- 实现`IUILogicStack`或`IUILogicFixed`接口；
- 应包含public无参构造函数。

此设计主要是为了方便项目对UI[逻辑](#界面逻辑)类的定制：

- 可根据项目的实际情况，自行设计UI[逻辑](#界面逻辑)类的继承关系；
- 方便项目对界面生命周期事件的封装、响应和二次委派处理。

推荐做法：

以实现`IUILogicBase`中`VisibleOperateType`属性和`OnCreate()`方法为例，[堆叠型](#堆叠型的界面)和[固定层级](#固定层级的界面)界面的共用基类的基类`UILogic`的写法为：

``` c#
public abstract class UILogicBase : IUILogicBase {
    ...
    protected virtual eUIVisibleOperateType VisibleOperateType { get { return eUIVisibleOperateType.LayerMask; } }
    ...
    protected virtual bool OnCreate(object para) { return true; }
    ...
	eUIVisibleOperateType IUILogicBase.VisibleOperateType { get { return VisibleOperateType; } }
	bool IUILogicBase.OnCreate(object para) {
	    // do something ...
	    return OnCreate(para);
	}
}
```

通过**显式实现接口**与重新定义同名**虚方法/虚属性**的方式，使得：

- [界面逻辑](#界面逻辑)类可以按需要来override相应的[生命周期方法](#生命周期方法)和[界面展示参数](#用于界面展示的参数)。
- 实现子类可响应[生命周期](#界面生命周期)事件的同时，基类也可根据项目需要，在显示实现接口的方法中进行相应处理，而子类在override方法或属性时无需关心是否需要调用`base`。

相应的[固定层级界面](#固定层级的界面)的基类：

``` c#
public abstract class UIFixedLogicBase : UILogicBase, IUILogicFixed {
    protected abstract int SortingOrderBias { get; }
    int IUILogicFixed.SortingOrderBias { get { return SortingOrderBias; } }
}
```

[堆叠型界面](#堆叠型的界面)的基类：

``` c#
public abstract class UIStackLogicBase : UILogicBase, IUILogicStack {

	protected virtual bool AllowMultiple { get { return false; } }
	protected abstract bool IsFullScreen { get; }
	protected abstract bool NewGroup { get; }
	protected virtual void OnGetFocus() { }
	
	protected virtual bool OnESC() { return true; }
	protected virtual void OnLoseFocus() { }

	bool IUILogicStack.AllowMultiple { get { return AllowMultiple; } }
	bool IUILogicStack.IsFullScreen { get { return IsFullScreen; } }
	bool IUILogicStack.NewGroup { get { return NewGroup; } }

	void IUIFocusable.OnGetFocus() { OnGetFocus(); }
	void IUIFocusable.OnLoseFocus() { OnLoseFocus(); }
	bool IUIFocusable.OnESC() { return OnESC(); }

}
```

## 界面逻辑初始代码生成

参考[初始界面逻辑代码生成工具](./README_code_generator.md)。

## 界面prefab资源合理性检查

参考[界面prefab合理性检查工具](./README_prefab_checker.md)。

# 典型功能界面及推荐做法

## 非交互型提示

特点：

- 显示在几乎所有其他UI之上；
- 整个项目内部通用；
- 通常是类似安卓Toast、有指向的气泡这类形式。

推荐做法：

- 使用`SortingOrderBias`值为负数（高层级）的[固定层级界面](#固定层级的界面)。
- 在此界面中建立数个各种形式提示的模板。
- [界面逻辑](#界面逻辑)根据需要，管理并创建模板实例，并填充需要展示的内容。
- 根据实际项目情况，选取事件、注册Agent等方式封装面向逻辑的功能接口。

## 加载进度条界面

特点：

- 需要在加载其他内容前，完成"进度条界面"的展示；
- 在"进度条界面"展示后，打开的界面应显示在进度条界面以下的更低层级的位置。

推荐做法：

- 使用`SortingOrderBias`值为负数（高层级）的[固定层级界面](#固定层级的界面)。
- 在[打开](#开启界面的api)"加载进度条界面"时，[侦听](#侦听被开启界面的生命周期)其[生命周期](#界面生命周期)中的`OnOpened`事件。
- 在"加载进度条界面"完成展示的响应中，开始加载其他内容。
- 在其他内容加载过程中，通过项目中的事件/数据驱动系统实时更新加载进度。
- 在其他内容加载完成后，[关闭](#关闭界面的api)"加载进度条界面"。

## 同一逻辑代码用于多个界面

此实现需要借助此UI管理器方案的数个特性：

- 借助[用于界面开启的参数](#用于界面开启的参数)来指定[界面节点](#界面节点)资源和[界面逻辑](#界面逻辑)类型。
- [界面逻辑](#界面逻辑)的[生命周期方法](#生命周期方法)中，`OnOpen(GameObject ui)`只给出了[界面节点](#界面节点)的`gameObject`实例，[界面逻辑](#界面逻辑)可以使用任何合理的方法来操控[界面节点](#界面节点)。

对于同一逻辑代码用于多个界面这个需求，在[开启界面的API](#开启界面的api)中使用[界面开启的参数](#用于界面开启的参数)将多个`id`不同`prefab_path`不同的界面，使用同一个`logic_type`即可。

但需要注意：

- 避免多个界面的prefab差异过大；
- 此`logic_type`指定的[界面逻辑](#界面逻辑)类，应尽量保证对多个相应的界面prefab的兼容性。
