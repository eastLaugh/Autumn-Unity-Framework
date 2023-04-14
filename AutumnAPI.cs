using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutumnFramework
{


    #region 特性
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class Bean : System.Attribute
    {
        public Type[] plugins = new Type[] { };
        // public virtual bool isMultiple => false;

    }
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class Beans : Bean
    {

        // override public bool isMultiple => true;
    }
    public class Config : Beans
    {

        public Config()
        {
            plugins = new Type[] { typeof(Configurationer) };   //安装Configurationer插件

        }
    }

    public class BeanInScene : Bean
    {
        public BeanInScene()
        {
            plugins = new Type[] { typeof(ObjectAutoSetup) };
        }
    }


    public class Beans_ObjectAutoSetup : Beans
    {
        public Beans_ObjectAutoSetup()
        {
            plugins = new Type[] { typeof(ObjectAutoSetup) };
        }
    }

    public class BeansInScene : Beans
    {
        public BeansInScene()
        {
            plugins = new Type[] { typeof(ObjectAutoSetup) };
        }
    }

    [System.AttributeUsage(AttributeTargets.Field)]
    public class Autowired : System.Attribute
    {
        public object msg;
        public Autowired() { }
        public Autowired(object msg)
        {
            this.msg = msg;
        }
    }
    #endregion
}