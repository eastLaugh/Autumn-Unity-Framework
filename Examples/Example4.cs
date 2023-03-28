using UnityEngine;
using AutumnFramework;
using System.Collections;
using System.Collections.Generic;



/*
假如你要实现，其中一个系统是一个单例，另外有一群Unity 组件，可能存在多个。



*/


//如果你添加的Bean是已经存在的对象，怎么办？   请使用Setup消息
[Beans]    //没错，Beans仍然是必不可少的。当然如果你确定是单例，也可以用Bean
public class Example4 : MonoBehaviour
{

    //注意Setup消息的API必须是 static
    public static IEnumerable Setup()
    {
        //注入了两个已经存在的Bean
        yield return GameObject.Find("Example4 Instance 1")?.GetComponent<Example4>();
        yield return GameObject.Find("Example4 Instance 2")?.GetComponent<Example4>();

    }

    public static bool Filter(object bean, object autowiredMsg)
    {
        if (autowiredMsg == null)
            return true;
        return "Example4 Instance " + autowiredMsg.ToString() == (bean as Example4).gameObject.name;
    }

    public void doSomething()
    {
        Debug.Log($"我是已经存在的{name}");
    }
}

[Bean]
public class 某个管控Gameobject的进程
{

    [Autowired]
    private List<Example4> Example4s;


    [Autowired("1")]
    private Example4 单个Example4;
    // Start 消息 晚于 Setup消息。所以尽情调用吧！
    private void Start()
    {
        try
        {
            Example4s[0]?.doSomething();    // 我是已经存在的Example4 Instance 1
            Example4s[1]?.doSomething();    // 我是已经存在的Example4 Instance 2
        }catch{
            //数组也许越界了？Bean也许没注入成功？如果你确保你的业务逻辑是正确的，你无需捕获异常

        }
    }
}