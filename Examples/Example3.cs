#define ENV_DEVELOPMENT
// #define ENV_PRODUCTION

using UnityEngine;
using AutumnFramework;


// [Config] 仅标记 ScritableObject 的子类
// Autumn 会扫描所有 ScriptableObject 文件，自动装配
// [Config] 等价于 [Beans(plugins = new System.Type[] { typeof(Configurationer) })]
[CreateAssetMenu(fileName = "Example3", menuName = "Autumn/Example3", order = 0)]
[Config]
public class Example3 : /**/ ScriptableObject {
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