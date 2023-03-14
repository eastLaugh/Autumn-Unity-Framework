using UnityEngine;
using Autumn;
using System.Linq;


[System.Serializable]
[Beans]     //注意这里是bean【S】!不是[Bean]而是[Beans]！
            //注意[Beans]是对[Bean]的扩充。因为有时候，我们不仅需要单例，也需要多例，
            //所以：
            // ① Beans 默认无Bean （而Bean是单例 及 默认有1个Bean）
            // ② 通过AutumnCore.PushBean()可以为其添加多个Bean，实现多例
public class 样本{

    public int 样本数据;
}


[Bean]  //作为一个系统，只需要单例就好，所以不加s
public class Example2的主系统:MonoBehaviour{
    [SerializeField]
    [Autowired]
    private 样本[] 样本集;  //对于Beans ，Autumn支持自动注入数组 
                          //如果找不到，则注入空数组(Autumn永远不会让null存在)

    private void Start() {
        //由于 样本是[Beans]即默认无Bean，所以我们需要添加一些
        样本 样本 = Autumn.Autumn.PushBean<样本>();
        样本.样本数据=1;   //第一个样本
        Autumn.Autumn.PushBean<样本>().样本数据=666;
        Autumn.Autumn.PushBean<样本>().样本数据=233;
    }

}


[Bean]
public class 副系统:MonoBehaviour{
    [SerializeField]
    [Autowired]
    private 样本 第一个样本;  //样本是Beans，如果不注入数组，则Autumn会自动抽取第一个Bean

    [Autowired]
    private 样本[] 样本数组;

}

// 现在，主系统和副系统通过Autumn来交互式共享数据。

