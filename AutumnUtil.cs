using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutumnFramework {
    [Serializable]
    public struct BeanConfig {
        [SerializeField]
        public List<object> Beans;

        public Entity BeanEntity;
        public enum Entity {
            Monobehaviour,
            Plain,  //开发中
            ScriptalObject,
        }

        public BeanConfig(List<object> beans, Entity entity) {
            Beans = beans;
            BeanEntity = entity;
        }

        public BeanConfig(object bean, Entity entity) : this(new List<object>() { bean }, entity) { }

        public BeanConfig(Entity entity) : this(new List<object>(), entity) { }

        public static BeanConfig.Entity GetEntity(Type beanType) {
            if (typeof(MonoBehaviour).IsAssignableFrom(beanType)) {
                return BeanConfig.Entity.Monobehaviour;
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(beanType)) {
                return BeanConfig.Entity.ScriptalObject;
            }
            else {
                return BeanConfig.Entity.Plain;
            }
        }
    }

    public class AutumnCoreException : System.Exception {
        public AutumnCoreException() { }
        public AutumnCoreException(string message) : base(message) { }
    }

    public static class AutumnUtil {
        public static bool IsEmptyListOrZeroArray(object obj) {
            if (obj == null) {
                return true;
            }
            if (obj is IList list && list.Count == 0) {
                return true;
            }
            if (obj is Array array && array.Length == 0) {
                return true;
            }
            return false;
        }
    }



}