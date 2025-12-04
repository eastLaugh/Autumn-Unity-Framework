using UnityEngine;
using AutumnFramework;

[Bean]
public class SingleSystem {
    [Autowired]   //自动装配
    private SingleMonoBehaviour singleMonoBehaviour;

    [Autowired]
    private Datalayer data;

    public SingleSystem() {
        // 不要使用构造函数
    }

    // Bean 的钩子函数 Start
    void Start() {
        // 已完成自动装配
    }

    // Bean 的钩子函数 Update
    void Update() {
    }
    public void InvokedByBean() {
        data.data++;
        Autumn.Harvest<SingleMonoBehaviour>().InvokedByHarvest();
    }
}

[Bean]   //MonoBehaviour Bean，会自动挂载在场景中，并设置 DontDestroyOnLoad
public class SingleMonoBehaviour : /**/ MonoBehaviour {

    [Autowired]
    private SingleSystem singleSystem;

    [Autowired]
    private Datalayer data;

    // Unity Monobehaviour Bean 不启用 Autumn 的钩子函数，因为已有同名 Unity 消息替代
    void Awake() {
        // 仅代表该 Unity Object 已被创建，与 Autumn 无关
    }
    void Start() {
        //已完成自动装配
        data.data++;
        singleSystem.InvokedByBean();
    }
    public void InvokedByHarvest() {
        data.data++;
    }

}

[Bean]
public class Datalayer {
    public int data;
}