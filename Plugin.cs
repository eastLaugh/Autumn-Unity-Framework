
using System;
using System.Collections;
using UnityEngine;
using System.Linq; // 配合 Linq 快速地为插件设置 Filter
using System.Collections.Generic;

namespace AutumnFramework
{
    public abstract class Plugin
    {
        protected Type beanType;
        protected virtual IEnumerable Setup()
        {
            return null;
        }
        protected virtual bool Filter(object bean, object autowiredMsg)
        {
            return true;
        }
    }

    // 用于[Config]的Bean的外置插件 （Autumn Built-In）
    public class Configurationer : Plugin
    {
        protected override IEnumerable Setup()
        {
            if (!typeof(ScriptableObject).IsAssignableFrom(beanType))
            {
                Debug.LogWarning(beanType.ToString() + "[Config]的最佳实践是用于ScriptableObject，而不是其他类。如果想要在其他类中实现类似[Config]的扩展，参考Autumn插件系统。");
            }
            var instances = Resources.LoadAll("", beanType);
            foreach (var instance in instances)
            {
                yield return instance;
            }
        }

        protected override bool Filter(object bean, object autowiredMsg)
        {
            if (autowiredMsg == null)
            {
                return true; //放行
            }
            else
            {
                return (bean as ScriptableObject).name == autowiredMsg.ToString();
            }
        }
    }


    public class ObjectAutoSetup : Plugin
    {

        protected override IEnumerable Setup()
        {
            //Debug.Log(UnityEngine.Object.FindObjectsOfType(typeof(VGF.SceneSystem.SceneLoader)).Length);
            //就是找不到，不知道为什么
            return UnityEngine.Object.FindObjectsOfType(beanType);
            // foreach(var instance in UnityEngine.Object.FindObjectsOfType(beanType)){
            //     yield return instance;
            // }
        }
    }
}