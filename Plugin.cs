
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
        protected object AutowiredMsg;
        public abstract IEnumerable Setup();
        public abstract bool Filter(object bean);
    }

    // 用于[Config]的Bean的外置插件 （Autumn Built-In）
    public class Configurationer : Plugin
    {
        public override IEnumerable Setup()
        {
            if(!typeof(ScriptableObject).IsAssignableFrom(beanType)){
                Debug.LogWarning("[Config]的最佳实践是用于ScriptableObject，而不是其他类。如果想要在其他类中实现类似[Config]的扩展，参考Autumn插件系统。");
            }
            var instances = Resources.LoadAll("", beanType);
            foreach (var instance in instances)
            {
                yield return instance;
            }
        }

        public override bool Filter(object bean)
        {
            if (AutowiredMsg == null)
            {
                return true; //放行
            }
            else
            {
                return (bean as ScriptableObject).name == AutowiredMsg.ToString();
            }
        }
    }
}