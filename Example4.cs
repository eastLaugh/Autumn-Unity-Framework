using UnityEngine;
using Autumn;
using System.Collections;
using System.Collections.Generic;


//如果你添加的Bean是已经存在的对象，怎么办？   请使用SETUP API!
[Beans]    //没错，Beans仍然是必不可少的。当然如果你确定是单例，也可以用Bean
public class Example4 :MonoBehaviour {
    
    //注意必须是 static
    public static IEnumerable Setup(){
        Debug.Log("Autumn Setup 消息 被调用！");

        yield return GameObject.Find("Example4 Instance 1").GetComponent<Example4>();
        yield return GameObject.Find("Example4 Instance 2").GetComponent<Example4>();
    }

    public void doSomething(){
        Debug.Log($"我是已经存在的{name}");
    }
}


[Bean]
public class 某个管控Gameobject的进程{

    [Autowired]
    private List<Example4> Example4s;

    private void Start(){
        // Start 消息 晚于 Setup消息。所以尽快调用吧！
        
        Example4s[0].doSomething();    // 我是已经存在的Example4 Instance 1
        Example4s[1].doSomething();    // 我是已经存在的Example4 Instance 2

    }
}