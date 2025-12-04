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


面向Unity的轻量级、渐进式、开箱即用、可扩展的IOC框架。

- 轻量级：AutumnCore核心代码仅占约400行。
- 渐进式：在你需要的任何地方、任何时机引入并享用。
- 开箱即用：引入Autumn后，零配置，零代码，通过[特性]与框架交互。
- 可扩展：你可以自己编写插件，来改变Autumn组织Bean的方式。

# 使用方法
直接将本仓库的所有文件放入Unity中即可。Examples目录及目录下文件仅供参考，可删除。

# 案例/快速上手

```Autumn-Unity-Framework/Examples/*.cs```

# 生命周期
![image](https://user-images.githubusercontent.com/39405923/225812457-45fe599f-9b87-472d-b236-082966cf1333.png)

# API

Autumn的API极其精简。大部分常用API均为C#特性，无需代码。

## 用于类的特性

- [Bean]

将一个类标记为Bean。启动后，Autumn将维护该类型为单例。

注意：Autumn会自动实例化该单例，无需手动实例化。因此你需要确保你的类型没有带参数的构造函数、私有的构造函数。

- [Beans]

Beans是Bean的容量扩增。支持多个实例。

注意：Autumn不会自动实例化给类型，也不会维护单例性。所以被Beans标记的类型实例，需要通过调用Autumn API手动添加。

- [Config]

Config的本质是加装了插件Configurationer的Beans。[Config]适用于ScriptableObject，游戏启动时，会自动调取Resources目录下的ScriptableObject并作为Bean添加。在装配时，可以指定对应的ScriptableObject注入。例如，使用[Autowired("生产环境")]装配名为生产环境的ScriptableObject。

## 用于字段的特性

- [Autowired]

自动装配。游戏运行时，Autumn以字段的类型为依据，自动为被[Autowired]标记的字段注入已有的Bean。如果该类型有多个实例（Beans），Autumn会尝试用列表或数组注入字段。

- [Autowired(object msg)]

在自动装配时传入msg，用于与插件、Filter API通讯。例如，装配[Config]标记的类型时，使用[Autowired("生产环境")]会装配名称为生产环境的ScriptableObject。

## 代码

- using AutumnFramework;

这是必须的操作。你也可以自动引用命名空间，以VSCODE为例，输入[Bean]后，提示Bean不存在，这时按下 Ctrl + . 选择第一项，将自动引入AutumnFramework命名空间。

- this.Bean();

将自己装配进Autumn中。


Example
```
[Beans]   //Autumn已经知道要管理System的多个实例啦
public class System:Monobehaviour{
  void Start(){
    this.Bean();   //告诉Autumn自己需要被管理
  }
  [Autowired]    //在任何想注入的地方注入
  private System[] systems;
}

```
如果你真的想零代码，连this.Bean();也不想输入，可以使用[Beans_ObjectAutoSetup]标记类型，它本质上是加装了插件的Beans，它会自动调用UnityEngine.FindObjectsOfType来查找。不过，不推荐使用，因为它是不可控的。

- this.UnBean();

将自己取消装配。
