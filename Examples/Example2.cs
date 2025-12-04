using UnityEngine;
using AutumnFramework;
using System.Linq;


[System.Serializable]
[Beans]     //注意这里是bean【S】!不是[Bean]而是[Beans]！
            // [Beans]是对[Bean]的扩充。因为有时候，我们不仅需要单例，也需要多例，
            //所以：
            // ① Bean意味着单例，Beans意味着容纳多个Bean。
            // ② Beans不会自动创建Bean实例，需要手动NewBean()或PushBean()
            // ③ 通过Autumn.PushBean()可以为其添加多个Bean，实现多例
public class 样本{

    public int 样本数据=-1;
}


[Bean]  //作为一个系统，只需要单例就好，所以不加s
public class Example2的主系统:MonoBehaviour{
    [SerializeField]
    [Autowired]
    private 样本[] 样本集;  //对于Beans ，Autumn支持自动注入数组 
                          //如果找不到，则注入空数组(Autumn永远不会让null存在)

    private void Awake() {
        //由于 样本是[Beans]
        //所以 Autumn不会维持样本的单例，我们需要手动添加一些样本
        样本 样本 =Autumn.NewBean<样本>();
        样本.样本数据=1; 

        //链式
        Autumn.NewBean<样本>().样本数据=666;
        Autumn.NewBean<样本>().样本数据=233;
    }

}


[Bean]
public class 视图层:MonoBehaviour{
    [SerializeField]
    [Autowired]
    private 样本 第一个样本;  //样本是Beans，如果不注入数组，则Autumn会自动抽取第一个Bean

    [Autowired]
    private 样本[] 样本数组;  // {1 , 666 , 233}

}

// 主系统、视图层、数据层通过Autumn畅快地共享数据