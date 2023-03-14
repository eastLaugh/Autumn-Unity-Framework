using System;
using System.Collections.Generic;
using UnityEngine;

namespace Autumn
{
    [System.Serializable]
    public struct BeanConfig
    {
        [UnityEngine.SerializeField]
        public List<object> Beans;

        public Entity BeanEntity;
        public enum Entity
        {
            Monobehaviour,
            Plain,  //开发中
            ScriptalObject,
        }
        public BeanConfig(List<object> beans, Entity entity)
        {
            Beans = beans;
            BeanEntity = entity;
        }

        public BeanConfig(object bean,Entity entity):this(new List<object>(){bean},entity){}

        public BeanConfig(Entity entity):this(new List<object>(),entity){}
        
    }
    public static class Extension
    {
    }

        #region Attribute
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class Bean : System.Attribute {
        public virtual bool isMultiple=>false;
        public Plugin[] plugins;
    }
    public class Beans : Bean{
        override public bool isMultiple=>true;
    }
    public class Configuration:Beans{
        public Configuration(){
            //不应该在 Bean 与 Plugin 中之间建立通讯
            plugins = new Plugin[]{new Configurationer()};
        }
    }
[System.AttributeUsage(AttributeTargets.Field)]
    public class Autowired : System.Attribute { 
        public object msg;
        public Autowired(){}
        public Autowired(object msg){
            this.msg=msg;
        }
    }
    
    #endregion

    [System.Serializable]
    public class AutumnCoreException : System.Exception
    {
        public AutumnCoreException() { }
        public AutumnCoreException(string message) : base(message) { }
    }

}