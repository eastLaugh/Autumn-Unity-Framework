//插件系统
using UnityEngine;
using AutumnFramework;
using System.Collections;
using System;

[Bean(plugins = new Type[]{ typeof(Configurationer)})]
public class Example5 :ScriptableObject {
    
}


public class PluginExample : Plugin
{
    protected override bool Filter(object bean, object autowiredMsg)
    {
        return true;
    }

    protected override IEnumerable Setup()
    {
        return null;
    }
}