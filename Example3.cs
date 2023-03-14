using UnityEngine;
using Autumn;


[CreateAssetMenu(fileName = "Example3", menuName = "Autumn/Example3", order = 0)]
[Configuration]
public class Example3 : ScriptableObject {
    public float speed;

}

[Bean]
public class Main : MonoBehaviour{

    [Autowired]     //不指定名称则会默认装配第一个配置项，按照Unity读取资源的顺序
    public Example3 example;

    [Autowired("配置项1")]
    public Example3 这个值是配置项1的;
    [Autowired("临时配置")]  
    public Example3 随意在多个环境间切换配置;

    // [Autowired("生产环境")]  
    [Autowired("测试环境")]  
    public Example3 默认配置;

    // [Autowired("生产环境")]
    // private Example3 productConfig;

    public void Start(){
        
    }
}