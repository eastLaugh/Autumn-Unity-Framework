using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace AutumnFramework
{
    public static partial class Autumn
    {

        private static Type[] BeanTypes;
        public static Dictionary<Type, BeanConfig> IOC = new();

        private static Type[] types;

        private static bool isIOCInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        // Autumn 生命周期
        //  启动 Play
        //        ↓
        // Unity 原生 Awake消息   ← 可提前PushBean
        //        ↓
        //Unity 原生 OnEnable消息
        //        ↓
        static void Autumn唯一入口函数()
        {
            isIOCInitialized = false;
            //    ↓
            InitializeIOC();    //   →   Setup 消息  →  Autumn插件 Setup   →   Unity 原生Awake消息
            //    ↓
            isIOCInitialized = true;   // 此时所有Bean已生成，但尚未装配
            //    ↓         
            Autowired();      // → Autumn插件 Filter  
            //    ↓
            // Autumn Start 消息
            Call("Start");
            //    ↓
            CheckEmptywired(); 
            //     ↓
        }
            //     ↓
            // Unity 原生Start消息


        // 后续操作生命周期
        // PushBean()  →   Autumn Start 消息  →  Autowired

        public static void CheckEmptywired()
        {
            foreach (var fieldInfo in GetAttributedFieldsInfo<Autowired>())
            {
                foreach (var bean in GetBeans(fieldInfo.DeclaringType))
                {
                    if (AutumnUtil.IsEmptyListOrZeroArray(fieldInfo.GetValue(bean)))
                    {
                        Debug.LogWarning($"{fieldInfo.DeclaringType.FullName} . {fieldInfo} ← 装配为空或空数组或空列表");
                    }
                }
            }
        }
        public static void InitializeIOC()
        {
            #region 添加Bean操作
            types = Assembly.GetExecutingAssembly().GetTypes();
            BeanTypes = types.Where(t => t.GetCustomAttribute<Bean>() != null).ToArray();
            foreach (var beanType in BeanTypes)
            {
                BeanConfig beanConfig = CreateEmptyBeanConfig(beanType);
                if (beanType.GetCustomAttribute<Beans>() != null)
                {
                }
                else
                    PushBean(beanType);

                //调用Setup消息
                {
                    object returnValue = beanType.GetMethod("Setup", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(null, null);
                    if (returnValue is IEnumerable IEnumerableValue)
                    {
                        PushExistedBean(beanType, IEnumerableValue);
                    }
                }

                //Autumn插件
                {
                    Plugin[] plugins = beanType.GetCustomAttribute<Bean>().plugins;
                    if (plugins != null)
                        foreach (var plugin in plugins)
                        {
                            if (plugin.Setup(beanType) is IEnumerable IEnumerableValue)
                            {
                                PushExistedBean(beanType, IEnumerableValue);
                            }
                        }
                }

            }
            #endregion
        }



        public static void Autowired()
        {

            foreach (var fieldInfo in GetAttributedFieldsInfo<Autowired>())
            {
                Autowired autowired = fieldInfo.GetCustomAttribute<Autowired>();

                Type beanType;
                if (typeof(IEnumerable).IsAssignableFrom(fieldInfo.FieldType))
                {
                    //此为枚举类型
                }
                if (fieldInfo.FieldType.BaseType == typeof(Array))
                {
                    //Array
                    beanType = fieldInfo.FieldType.GetElementType();
                    var beans = Filter(GetBeans(beanType)).ToArray();
                    Array filledArray = Array.CreateInstance(beanType, beans.Length);
                    Array.Copy(beans, filledArray, beans.Length);
                    Assign(filledArray);
                }
                else if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    // List
                    beanType = fieldInfo.FieldType.GetGenericArguments()[0];
                    var beans = Filter(GetBeans(beanType)).ToArray();
                    Array filledArray = Array.CreateInstance(beanType, beans.Length);
                    Array.Copy(beans, filledArray, beans.Length);
                    object v = Activator.CreateInstance(typeof(List<>).MakeGenericType(beanType), filledArray);
                    Assign(v);
                }
                else
                {
                    beanType = fieldInfo.FieldType;
                    object[] beans = Filter(GetBeans(beanType)).ToArray();
                    if (beans.Length > 0)
                        Assign(beans[0]);
                    else
                        Assign(null);
                }
                IEnumerable<object> Filter(IEnumerable<object> origin)
                {
                    var plugins = beanType.GetCustomAttribute<Bean>().plugins;
                    if (plugins != null)
                    {
                        foreach (Plugin plugin in beanType.GetCustomAttribute<Bean>().plugins)
                        {
                            foreach (var item in plugin.Filter(beanType, autowired.msg, origin))
                            {
                                yield return item;
                            }
                        }
                    }
                    else
                        foreach (var item in origin)
                        {
                            yield return item;
                        }

                }

                void Assign(object value)
                {



                    foreach (var obj in GetBeans(fieldInfo.DeclaringType))
                    {
                        if (obj != null)
                            fieldInfo.SetValue(obj, value);
                        else
                            throw new AutumnCoreException($"{fieldInfo.DeclaringType.FullName} . {fieldInfo} 装配失败,因为{obj}为空");
                    }
                }

            }

        }
        public static TBean PushBean<TBean>() where TBean : class
        {
            return PushBean(typeof(TBean)) as TBean;
        }
        public static object PushBean(Type beanType)
        {
            object chain = null;
            if (IOC.TryGetValue(beanType, out BeanConfig beanConfig))
            {
            }
            else
            {
                beanConfig = CreateEmptyBeanConfig(beanType);
            }

            switch (beanConfig.BeanEntity)
            {
                case BeanConfig.Entity.Monobehaviour:
                    GameObject g = new GameObject(beanType.Name);
                    MonoBehaviour.DontDestroyOnLoad(g);

                    beanConfig.Beans.Add(chain = g.AddComponent(beanType));
                    break;
                case BeanConfig.Entity.ScriptalObject:

                    break;

                case BeanConfig.Entity.Plain:
                default:
                    var instance = Activator.CreateInstance(beanType);
                    beanConfig.Beans.Add(chain = instance);
                    if (isIOCInitialized)
                        instance.GetType().GetMethod("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(instance, null);
                    break;
            }
            if (isIOCInitialized)
            {
                Autowired();
            }
            return chain;
        }


        public static object PushExistedBean(Type beanType, object existedBean, bool autowired = true)
        {
            if (IOC.TryGetValue(beanType, out BeanConfig beanConfig))
            {
            }
            else
            {
                beanConfig = CreateEmptyBeanConfig(beanType);
            }
            beanConfig.Beans.Add(existedBean);
            if (isIOCInitialized)
            {
                if (autowired)
                    Autowired();
            }
            return existedBean;
        }
        public static void PushExistedBean(Type beanType, IEnumerable existedBean)
        {
            foreach (var bean in existedBean)
            {
                PushExistedBean(beanType, bean, false);
            }
            if (isIOCInitialized)
            {
                Autowired();
            }
        }
        private static BeanConfig CreateEmptyBeanConfig(Type beanType)
        {
            BeanConfig chain;
            if (typeof(MonoBehaviour).IsAssignableFrom(beanType))
            {
                IOC.Add(beanType, chain = new BeanConfig(BeanConfig.Entity.Monobehaviour));
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(beanType))
            {
                IOC.Add(beanType, chain = new BeanConfig(BeanConfig.Entity.ScriptalObject));
            }
            else
            {
                IOC.Add(beanType, chain = new BeanConfig(BeanConfig.Entity.Plain));
            }
            return chain;
        }
        public static void Call(String functionName)
        {
            #region Autumn消息 Start
            foreach (KeyValuePair<System.Type, BeanConfig> kvp in IOC)
            {
                switch (kvp.Value.BeanEntity)
                {
                    case BeanConfig.Entity.Monobehaviour:
                        //Autumn消息不支持MonoBehaviour
                        break;

                    default:
                        kvp.Key.GetMethod(functionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(kvp.Value.Beans.First(), null);
                        break;

                }
            }
            #endregion
        }

        private static IEnumerable<FieldInfo> GetAttributedFieldsInfo<TAttribute>() where TAttribute : Attribute
        {
            return types.SelectMany(type => type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)).Where(field => field.GetCustomAttribute<TAttribute>() != null);
        }
        public static T Harvest<T>() where T : class
        {
            return Harvest(typeof(T)) as T;
        }
        public static object Harvest(Type type)
        {
            if (IOC.TryGetValue(type, out BeanConfig instance))
            {
                return instance.Beans.Count > 0 ? instance.Beans.First() : null;
            }
            else
            {
                throw new AutumnCoreException($"{type.FullName} 未添加[Bean]特性");
            }
        }
        public static List<T> GetBeans<T>() where T : class
        {
            return Harvest(typeof(T)) as List<T>;
        }
        public static List<object> GetBeans(Type type)
        {
            if (IOC.TryGetValue(type, out BeanConfig instance))
            {
                return instance.Beans;
            }
            else
            {
                throw new AutumnCoreException($"{type.FullName} 未添加[Bean]特性");
            }
        }
    }
}