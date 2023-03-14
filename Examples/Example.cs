/*
Autumn IOC Framework Based on Unity
面向UNITY的轻量级、渐进式、开箱即用的IOC框架。

轻量级：AutumnCore核心代码仅占约400行。
渐进式：在你需要的任何地方、任何时机引入并享用。
开箱即用：引入Autumn后，零配置，零代码，通过[特性]与框架交互。
可扩展：你可以自己编写插件，来改变Autumn组织Bean的方式。

作者邮箱：east_laugh@qq.com
作者github: https://github.com/eastLaugh
*/

using UnityEngine;
using AutumnFramework;

[Bean]    //将某个系统类标记为一个[Bean]    Bean意味着：单例、全局、豆角(?)
public class SingleSystem
{
    [Autowired]   //这是自动装配，是用于装配其他Bean的
    private SingleMonoBehaviour singleMonoBehaviour;
    
    [Autowired]   //同上
    private 数据层 数据层;

    public SingleSystem()
    {
        Debug.Log("SingleSystem被创建，这时还没有完成自动装配");
    }

    // 所有Bean兼容Unity消息（Start、Update）
    void Start()
    {
        Debug.Log("当执行此Start内，已完成自动装配");
    }
    void Update()
    {
        //还有Update也支持！
    }
    public void 通过自动装配从别处调用此函数_实现解耦()
    {
        数据层.数据++;
        Debug.Log("通过自动装配从别处调用此函数_实现解耦");
        Autumn.Harvest<SingleMonoBehaviour>().通过Harvest_API从别处调用_实现解耦();
    }
}

[Bean]   //将一个MonoBehaviour子类标记为Bean，他会自动出现在场景里！
public class SingleMonoBehaviour : /**/  MonoBehaviour   /**/
{

    [Autowired]
    private SingleSystem singleSystem;

    [Autowired]
    private 数据层 数据层;

    private void Awake() {
        //这里还没完成自动装配，不能调用其他Bean
    }
    private void Start()
    {
        //哈哈，这里已经完成了自动装配。随意调用吧！
        数据层.数据++;
        Debug.Log("Monobehaviour原生Start，会比Autumn提供的Start较晚执行");
        singleSystem.通过自动装配从别处调用此函数_实现解耦();


    }
    public void 通过Harvest_API从别处调用_实现解耦()
    {
        数据层.数据++;
        Debug.Log("通过Harvest_API从别处调用_实现解耦");
        Debug.Log(数据层.数据++);
    }

}

[Bean]
public class 数据层{
    public int 数据;
}


/*

如你所见，Autumn 成功为这三个紧密的系统解了耦！
尽情酸爽的调用吧！

*/

