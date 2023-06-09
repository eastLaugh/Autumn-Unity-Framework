
using System;
using System.Collections;
using UnityEngine;
using System.Linq; // 配合 Linq 快速地为插件设置 Filter
using System.Collections.Generic;

namespace AutumnFramework
{
    public abstract class Plugin
    {
        internal AutumnConfig autumnConfig;
        internal Type beanType;
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
                Debug.LogWarning(beanType.ToString() + "[Config]的最佳实践是用于ScriptableObject，而不是其他类。");
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
            UnityEngine.Object[] objects = UnityEngine.Object.FindObjectsOfType(beanType);
            if (objects == null || objects.Length == 0)
            {
                // throw new AutumnCoreException(String.Format(autumnConfig.场景丢失Bean, beanType));
                Debug.LogError(String.Format(autumnConfig.场景丢失Bean, beanType));
            }
            return objects;
        }
    }
}