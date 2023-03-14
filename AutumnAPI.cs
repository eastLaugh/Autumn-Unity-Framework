using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutumnFramework
{
    public static partial class Autumn
    {

    }
    #region 特性
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class Bean : System.Attribute
    {
        public virtual bool isMultiple => false;
        public Plugin[] plugins;
    }
    public class Beans : Bean
    {
        override public bool isMultiple => true;
    }
    public class Config : Beans
    {
        public Config()
        {
            //不应该在 Bean 与 Plugin 中之间建立通讯
            plugins = new Plugin[] { new Configurationer() };
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
