<p align="center">
  <h3 align="center">🍃🍃🍃</h3>
  <h3 align="center">🍃🍃🍃</h3>
  <h3 align="center">🍃🍃🍃</h3>
</p>
<p align="center">
  <h3 align="center">Autumn</h3>
</p>
<p align="center">
  <h3 align="center">千秋</h3>
</p>  

（破坏性更新阶段……在发布正式版本前，请勿投入正式使用）

面向Unity的轻量级、渐进式、开箱即用、可扩展的IOC框架。

- 轻量级：AutumnCore核心代码仅占约400行。
- 渐进式：在你需要的任何地方、任何时机引入并享用。
- 开箱即用：引入Autumn后，零配置，零代码，通过[特性]与框架交互。
- 可扩展：你可以自己编写插件，来改变Autumn组织Bean的方式。

作者邮箱：east_laugh@qq.com

# 案例/快速上手

```Autumn-Unity-Framework/Examples/*.cs```

# 生命周期

# API

Autumn的API极其精简。大部分常用API均为C#特性，无需代码。

## 用于类的特性

- [Bean]

将一个类标记为Bean。启动后，Autumn会自动实例化该类，适合单例系统使用。

- [Beans]

Beans是Bean的容量扩增，支持多个实例。通过Setup API或PushBean等操作手动添加实例。

- [Config]

Config的本质是加装了插件Configurationer的Beans，详见Autumn的插件系统。[Config]适用于ScriptableObject，游戏启动时，会自动调取Resources目录下的ScriptableObject并作为Bean添加。在装配时，可以指定装配msg，[Config]的插件Configurationer会查找符合msg作为对象名的Bean注入。例如，使用[Autowired("生产环境")]装配名为生产环境的ScriptableObject。

## 用于字段的特性

- [Autowired]

自动装配。游戏运行时，Autumn以字段的类型为依据，自动为被[Autowired]标记的字段注入已有的Bean。如果有多个Bean，Autumn会尝试用列表或数组注入字段。

_ [Autowired(object msg)]

在自动装配时传入msg，用于与插件、Filter API通讯。例如，装配[Config]标记的类型时，使用[Autowired("生产环境")]会装配名称为生产环境的ScriptableObject，详见[Config] API。

## 代码

- Autumn.PushBean()

这是通过代码为一个类型强制添加Bean。参数为类型的构造函数。

提供泛型版本，省略类型参数。

不推荐使用new创建实例，而是采用PushBean。如果你想将一个已经存在的实例添加到Bean，请使用下面的API ：

- Autumn.PushExistedBean()

这是通过代码，将一个已经存在的实例作为该类型的Bean。参数为实例。

- Autumn.Autowired()

手动进行一次自动装配。
