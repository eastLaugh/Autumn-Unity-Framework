using UnityEngine;
using AutumnFramework;
using System.Collections;
using System.Collections.Generic;
using System;
//插件系统

[Bean(plugins = new Type[]{ typeof(Configurationer)})]
public class Example5 :MonoBehaviour {
    
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