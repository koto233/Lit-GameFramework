## 依赖管理规范

### 允许使用 `GameRoot.I.Get<T>()` 的场景：
1. UI组件、View层、MonoBehaviour
2. 编辑器工具、调试代码
3. 原型阶段快速验证

### 禁止使用 `GameRoot.I.Get<T>()` 的场景：
1. Manager类、Service类、System类
2. 业务逻辑层、领域模型
3. 需要单元测试的代码

### 正确的做法（对于禁止使用的场景）：
```csharp
// 使用构造函数注入
public class ShopService : IShopService
{
    private readonly IInventory _inventory;
    private readonly IEconomy _economy;
    
    public ShopService(IInventory inventory, IEconomy economy)
    {
        _inventory = inventory;
        _economy = economy;
    }
}


一、核心根节点
GameRoot

身份：应用根 / 生命周期锚点

职责：

管理全局与场景级 LifetimeScope

提供统一访问入口 Get<T>()

不承载任何业务逻辑

禁止：

❌ 不实现任何 Service / System / Controller

❌ 不参与游戏流程

❌ 不缓存业务状态

GameRoot 只回答一个问题：
“现在还能不能活着？”

LifetimeScope

身份：对象生命周期容器

职责：

注册实例

管理 Dispose

提供 Resolve / TryResolve

允许：

显式注册实例

显式 Resolve

禁止：
❌ 业务语义

不知道 Battle / UI / Player

不参与游戏流程判断

❌ 表现层感知

不依赖 MonoBehaviour


LifetimeScope 不知道“这是什么”，
只知道“什么时候该死”。

二、一级架构角色（最重要）
Service

身份：能力提供者（横向）

职责：

为多个 Controller / System 提供能力

通常无业务状态，或只有技术状态

生命周期通常为 Global

示例：

UIService

AudioService

InputService

SaveService

允许：

被多个模块调用

依赖其他 Service

禁止：

❌ 控制游戏流程

❌ 承载业务规则

❌ 直接操纵 Scene 逻辑

Service 回答的是：
“我能为你做什么”

System

身份：业务规则内核

职责：

实现游戏规则 / 算法 / 状态演进

与 UI、Scene、MonoBehaviour 解耦

可独立测试

示例：

BattleSystem

InventorySystem

QuestSystem

允许：

维护业务状态

被 Controller 调用

禁止：

❌ 引用 UI / MonoBehaviour

❌ 直接访问 GameRoot

❌ 处理输入或展示

System 回答的是：
“规则是什么”

Controller

身份：流程协调者（纵向）

职责：

协调 Service 与 System

驱动场景级流程

生命周期通常为 Scene

示例：

BattleController

MainMenuController

LoginController

允许：

调用 UIService

调用 System

持有状态

禁止：

❌ 实现核心规则

❌ 成为全局单例

❌ 承担长期资源

Controller 回答的是：
“现在该发生什么”

🎨 三、表现层角色
UIWindow / View

身份：纯表现层

职责：

显示数据

接收用户输入

转发事件

允许：

引用 Controller

触发回调

禁止：

❌ 包含业务规则

❌ 操纵 System 状态

❌ 访问 GameRoot

UI 只应该知道：
“用户点了什么”

⚙️ 四、辅助角色（非架构核心）
Model

身份：数据结构

职责：

存储数据

不包含行为（或极少）

示例：

PlayerData

ItemData

Config

身份：只读配置

职责：

表数据

常量定义

🚫 五、禁止角色（框架级）
❌ Manager

状态：框架级禁用

原因：

职责模糊

容易膨胀

无边界约束

例外：

只能作为 private 内部类

不得暴露

不参与注入

🧭 六、命名即约束（强制规则）

类名必须能回答一句话

名称	能回答的问题
Service	我能提供什么能力
System	规则是什么
Controller	现在该做什么
View / Window	我该怎么显示
Model	数据长什么样