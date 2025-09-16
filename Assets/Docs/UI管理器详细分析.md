# W-Framework UI管理器详细分析

## 概述

W-Framework的UI管理器是整个框架的核心组件，提供了完整的UI生命周期管理、层级控制、焦点管理和分组操作等功能。本文档深入分析其设计原理和实现机制。

## UI管理器架构

### 核心组件

1. **UIManager** - 主管理器，提供对外接口
2. **Processor** - 内部处理器，负责UI实例管理
3. **FocusMgr** - 焦点管理器，处理UI焦点控制
4. **UIInstanceBase** - UI实例基类
   - **UIInstanceStack** - 堆叠UI实例
   - **UIInstanceFixed** - 固定UI实例

### 设计模式

- **工厂模式**: 通过UILoader创建UI实例
- **对象池模式**: Logic实例和UIInstance的缓存复用
- **状态机模式**: UI生命周期状态管理
- **观察者模式**: 焦点变化通知机制

## UI生命周期详细分析

### 完整生命周期流程图

```mermaid
flowchart TD
    A[开始打开UI] --> B{检查参数有效性}
    B -->|无效| Z[失败返回]
    B -->|有效| C[获取Logic实例]

    C --> D[调用Logic.OnCreate]
    D --> E{OnCreate返回结果}
    E -->|false| F[缓存Logic实例]
    F --> Z
    E -->|true| G[暂停焦点变化分发]

    G --> H[创建UIInstance]
    H --> I[异步并行处理]

    I --> J[准备阶段检查]
    I --> K[异步加载Prefab]

    J --> L{需要准备?}
    L -->|是| M[启动超时检查]
    L -->|否| N[准备完成:成功]
    M --> O[调用Logic.OnPrepareExecute]
    O --> P[等待准备结果]
    P --> Q{准备结果}
    Q -->|成功| N
    Q -->|失败| R[准备完成:失败]
    Q -->|超时| S[准备完成:超时]

    K --> T{Prefab加载结果}
    T -->|失败| U[设置加载结果:-1]
    T -->|成功| V[设置加载结果:1]
    U --> W[关闭UI]

    V --> X[初始化UI组件]
    X --> Y[设置父节点和变换]
    Y --> AA[尝试打开UI]

    AA --> BB{准备和加载都完成?}
    BB -->|否| CC[等待另一个完成]
    BB -->|是| DD{准备成功且加载成功?}
    DD -->|否| EE[关闭UI]
    DD -->|是| FF[设置状态:已打开]

    FF --> GG[调用Logic.OnOpen]
    GG --> HH[触发事件:OnOpened]
    HH --> II{当前应该显示?}
    II -->|是| JJ[显示UI]
    II -->|否| KK[隐藏UI]

    JJ --> LL[调用Logic.OnShow]
    LL --> MM[触发事件:OnShown]
    MM --> NN[处理冲突和层级]

    KK --> NN
    NN --> OO[恢复焦点变化分发]
    OO --> PP[UI打开完成]

    %% 隐藏流程
    PP --> QQ[UI运行中]
    QQ --> RR{触发隐藏?}
    RR -->|是| SS[设置showing:false]
    SS --> TT[调用UI.DoHide]
    TT --> UU[调用Logic.OnHide]
    UU --> VV[触发事件:OnHided]
    VV --> QQ

    %% 恢复显示流程
    RR -->|恢复显示| WW[设置showing:true]
    WW --> XX[调用UI.DoShow]
    XX --> YY[调用Logic.OnShow]
    YY --> ZZ[触发事件:OnShown]
    ZZ --> QQ

    %% 关闭流程
    QQ --> AAA{触发关闭?}
    AAA -->|是| BBB[设置状态:已关闭]
    BBB --> CCC{当前在显示?}
    CCC -->|是| DDD[调用UI.DoHide]
    DDD --> EEE[调用Logic.OnHide]
    EEE --> FFF[调用Logic.OnClose]
    CCC -->|否| FFF
    FFF --> GGG[清理UI组件]
    GGG --> HHH[卸载GameObject]
    HHH --> III[触发事件:OnClosed]
    III --> JJJ[调用Logic.OnTerminated]
    JJJ --> KKK[触发事件:OnTerminated]
    KKK --> LLL[缓存Logic实例]
    LLL --> MMM[缓存UIInstance]
    MMM --> NNN[UI关闭完成]

    EE --> W
    CC --> BB

    style A fill:#e1f5fe
    style PP fill:#c8e6c9
    style NNN fill:#ffcdd2
    style Z fill:#ffcdd2
```

### 状态机详解

UI实例在生命周期中会经历以下状态：

1. **None** - 初始状态
2. **Preparing** - 准备中（异步加载和准备阶段）
3. **Opened** - 已打开（正常运行状态）
4. **Closed** - 已关闭（终止状态）

### 异步处理机制

UI管理器采用双异步并行处理：

```mermaid
sequenceDiagram
    participant UI as UIInstance
    participant Logic as UILogic
    participant Loader as UILoader

    UI->>+Logic: OnPrepareCheck()
    Logic-->>UI: 返回是否需要准备

    par 准备阶段
        UI->>+Logic: OnPrepareExecute()
        Logic-->>-UI: 准备完成
    and 加载阶段
        UI->>+Loader: LoadUIObject()
        Loader-->>-UI: 返回GameObject
    end

    UI->>UI: TryOpenUI()
    Note over UI: 当两个阶段都完成时才打开
```

## UI层级管理系统

### 层级架构

W-Framework将UI分为两大类型：

1. **Stack UI (堆叠UI)**
   - 支持后进先出的堆叠管理
   - 支持全屏UI自动隐藏下层UI
   - 支持分组管理

2. **Fixed UI (固定UI)**
   - 固定在特定层级
   - 不受堆叠影响
   - 适用于HUD、工具栏等

### 排序规则

```mermaid
graph TB
    subgraph "UI层级结构"
        direction TB
        A[Fixed UI - Top Layer<br/>SortingOrder: Max - Bias]
        B[Stack UI - Layer N<br/>Index: N]
        C[Stack UI - Layer N-1<br/>Index: N-1]
        D[Stack UI - Layer 1<br/>Index: 1]
        E[Stack UI - Layer 0<br/>Index: 0]
        F[Fixed UI - Bottom Layer<br/>SortingOrder: Min + Bias]

        A --> B
        B --> C
        C --> D
        D --> E
        E --> F
    end

%%    subgraph "计算公式"
%%        G[Stack UI:<br/>SortingOrder = Min + Range × (Index + 2)<br/>PosZ = FromZ - Interval × (Index + 2)]
%%        H[Fixed UI (负Bias):<br/>SortingOrder = Max - Range + Bias<br/>PosZ = ToZ - Bias × Interval / Range]
%%        I[Fixed UI (正Bias):<br/>SortingOrder = Min + Bias<br/>PosZ = FromZ - Bias × Interval / Range]
%%    end
```

### 层级冲突处理

1. **互斥组冲突**: 相同MutexGroup的UI只能存在一个
2. **重复ID冲突**: 根据AllowMultiple设置决定是否关闭旧UI
3. **全屏UI处理**: 全屏UI会自动隐藏下层UI

## UI分组管理

### 分组机制

```mermaid
graph LR
    subgraph "UI分组示例"
        A[主界面 - Group1] --> B[商店界面 - Group1]
        B --> C[商品详情 - Group1]

        D[设置界面 - Group2] --> E[音频设置 - Group2]

        F[独立弹窗 - Group3]
    end

    subgraph "分组规则"
        G[NewGroup = true<br/>创建新分组]
        H[NewGroup = false<br/>继承上层分组]
    end
```

### 分组操作

- **OpenSingle**: 只关闭指定UI
- **CloseGroup**: 关闭整个分组
- **自动分组**: 根据NewGroup属性自动分配分组

## 焦点管理系统

### 焦点管理架构

```mermaid
classDiagram
    class FocusMgr {
        +IUIFocusable Current
        +KeyedPriorityQueue~IUIFocusable~ mFocuses
        +Dictionary~IUIDynamicFocusable~ mDynamicAgents
        +AddFocusable(obj, order) bool
        +RemoveFocusable(obj) bool
        +HoldDispatchFocusChange()
        +TryDispatchFocusChange()
    }

    class IUIFocusable {
        <<interface>>
        +OnGetFocus()
        +OnLoseFocus()
        +OnESC()
    }

    class IUIDynamicFocusable {
        <<interface>>
        +SetDynamicFocusAgent(agent)
    }

    class DynamicFocusAgent {
        +RequireFocus() bool
        +ReleaseFocus() bool
    }

    FocusMgr --> IUIFocusable
    FocusMgr --> IUIDynamicFocusable
    FocusMgr --> DynamicFocusAgent
    IUIDynamicFocusable --> DynamicFocusAgent
```

### 焦点优先级

- 基于Priority Queue实现
- SortingOrder越高，焦点优先级越高
- 支持动态焦点申请和释放

## 性能优化特性

### 1. 对象池机制

```csharp
// Logic实例缓存
private static Dictionary<Type, Queue<IUILogicBase>> s_logic_cache;

// UIInstance实例缓存
private static LinkedList<T> s_caches = new LinkedList<T>();
```

### 2. 异步加载

- 并行处理准备和加载阶段
- 避免阻塞主线程
- 支持超时机制

### 3. 延迟计算

- SortingOrder和PositionZ按需计算
- 屏幕变化时才重新计算位置

### 4. 内存管理

- 自动资源清理
- GameObject及时卸载
- Logic实例复用

## 错误处理机制

### 异常安全

```csharp
try {
    Logic.OnCreate(parameter);
} catch (Exception e) {
    Debug.LogException(e);
}
```

### 容错设计

- 加载失败自动清理
- 准备超时自动关闭
- 异常不影响其他UI

## API设计亮点

### 1. 链式调用

```csharp
UIManager.OpenWithHandler(handler, "dialog", parameter);
```

### 2. 泛型约束

```csharp
private abstract class UIInstanceBase<T, U> : UIInstanceBase
    where T : UIInstanceBase<T, U>
    where U : IUILogicBase
```

### 3. 接口分离

- `IUILogicStack` - 堆叠UI接口
- `IUILogicFixed` - 固定UI接口
- `IUIFocusable` - 焦点接口
- `IUIDynamicFocusable` - 动态焦点接口

## 总结

W-Framework的UI管理器通过以下设计实现了高效的UI管理：

### 核心优势

1. **完整的生命周期管理** - 从创建到销毁的全流程控制
2. **智能层级系统** - 自动排序和冲突处理
3. **灵活的分组机制** - 支持复杂的UI组织结构
4. **高效的焦点管理** - 基于优先级队列的焦点控制
5. **异步加载机制** - 避免UI加载阻塞
6. **对象池优化** - 提升性能和内存效率
7. **异常安全设计** - 保证系统稳定性

### 设计哲学

- **职责分离** - 每个组件都有明确的职责
- **扩展性** - 通过接口和继承支持定制
- **性能优先** - 内存池和异步加载优化
- **开发友好** - 简洁的API和完善的错误处理

这使得W-Framework的UI管理器成为了一个功能强大、性能优异且易于使用的UI管理解决方案。