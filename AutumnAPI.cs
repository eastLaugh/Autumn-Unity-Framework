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
        public virtual bool isMultiple => false;
        public virtual Type[] plugins { get; } = new Type[] { };
    }
    public class Beans : Bean
    {
        override public bool isMultiple => true;
    }
    public class Config : Beans 
    {
        public override Type[] plugins { get; } = new Type[] { typeof(Configurationer) };   //安装Configurationer插件
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