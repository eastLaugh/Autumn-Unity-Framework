//先不用在意这里！
#define ENV_DEVELOPMENT
// #define ENV_PRODUCTION

using UnityEngine;
using AutumnFramework;


// 把所有数据放在场景中是囊肿的。我们现在，需要更加持久化的数据存储方式。

[CreateAssetMenu(fileName = "Example3", menuName = "Autumn/Example3", order = 0)]
[Config]  // [Config]是特殊的[Beans]。它利用Autumn的插件系统，二次开发了Beans！事实上，[Config]比[Beans]的唯一区别就是多安装了一个插件。

// 注意[Config]必须标记ScritableObject的子类。
// Autumn会从本地已有的ScriptableObject自动注入并装配。
public class Example3 : /* */ ScriptableObject {
    public float speed;
}

[Bean]
public class Main : MonoBehaviour {

    [Autowired]
    public Example3 example;

    [Autowired("配置项1")]
    public Example3 config1;
    [Autowired("临时配置")]
    public Example3 configForTemp;

#if ENV_DEVELOPMENT
    [Autowired("开发环境")]
#elif ENV_PRODUCTION
    [Autowired("生产环境")]
#endif
    public Example3 config;   //使用开头的宏来便捷的切换配置

}