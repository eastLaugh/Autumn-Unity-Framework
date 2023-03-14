
using System;
using System.Collections;
using UnityEngine;
using System.Linq; // 配合 Chat gpt 和 Linq快速地为插件设置Filter
using System.Collections.Generic;

namespace Autumn
{
    public abstract class Plugin
    {
        public abstract IEnumerable Setup(Type beanType);
        public abstract IEnumerable Filter(Type beanType, object msg, IEnumerable<object> beans);
    }

    // C# 在每次获取特性实例时会构建多个特性，同时构建多个特性实例，所以Autumn不确保插件实例的唯一性。你在内部设置的非静态成员的信息是不会保存的。
    // 简单来说，你你必须把 Plugin的子类 当作静态类用。
    // 注意：Plugin不能添加[Bean]，因为它与Autumn本身有关，就像你永远无法抓着自己的头发上天
    public class Configurationer : Plugin
    {
        public override IEnumerable Setup(Type beanType)
        {
            var instances = Resources.LoadAll("", beanType);
            foreach (var instance in instances)
            {
                yield return instance;
            }
        }

        public override IEnumerable Filter(Type beanType, object msg, IEnumerable<object> beans)
        {
            if (msg == null)
            {
                return beans;
            }
            else
            {
                return beans.Where(bean => (bean as ScriptableObject).name == msg.ToString());
            }
        }
    }
}