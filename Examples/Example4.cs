//插件系统
using UnityEngine;
using AutumnFramework;
using System.Collections;

[Beans(plugins = new System.Type[] { typeof(PluginExample) })]
public class Example4 : ScriptableObject {
}

public class PluginExample : Plugin {
    protected override bool Filter(object bean, object autowiredMsg) {
        return true;
    }

    protected override IEnumerable Setup() {
        return null;
    }
}