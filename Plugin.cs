
using System;
using System.Collections;
using UnityEngine;

namespace AutumnFramework {
    public abstract class Plugin {
        internal AutumnConfig autumnConfig;
        internal Type beanType;

        /// <summary>
        /// 自定义装配逻辑，可以额外装配 Bean 到 IOC 中
        /// </summary>
        /// <returns>额外装配的 Bean 迭代器</returns>
        protected virtual IEnumerable Setup() {
            return null;
        }
        /// <summary>
        /// 过滤 Beans , 选择性实现 Autowired 特性
        /// </summary>
        /// <param name="bean"> 当前 Bean </param>
        /// <param name="autowiredMsg"> Autowired 上下文 </param>
        /// <returns>该 Bean 是否符合装配条件</returns>
        protected virtual bool Filter(object bean, object autowiredMsg) {
            return true;
        }
    }

    // 用于[Config]的Bean的外置插件 （Autumn Built-In）
    public class Configurationer : Plugin {
        protected override IEnumerable Setup() {
            if (!typeof(ScriptableObject).IsAssignableFrom(beanType)) {
                Debug.LogWarning(beanType.ToString() + "[Config]的最佳实践是用于ScriptableObject，而不是其他类。");
            }
            var instances = Resources.LoadAll("", beanType);
            foreach (var instance in instances) {
                yield return instance;
            }
        }

        protected override bool Filter(object bean, object autowiredMsg) {
            if (autowiredMsg == null) {
                return true;
            } else {
                return (bean as ScriptableObject).name == autowiredMsg.ToString();
            }
        }
    }


    public class ObjectAutoSetup : Plugin {
        protected override IEnumerable Setup() {
            UnityEngine.Object[] objects = UnityEngine.Object.FindObjectsOfType(beanType);
            if (objects == null || objects.Length == 0) {
                // throw new AutumnCoreException(String.Format(autumnConfig.场景丢失Bean, beanType));
                Debug.LogError(string.Format(autumnConfig.场景丢失Bean, beanType));
            }
            return objects;
        }
    }
}