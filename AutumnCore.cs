using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace AutumnFramework
{

    public static class Autumn
    {
        [Autowired]
        private static AutumnConfig autumnConfig;

        private static Type[] BeanTypes;

        private static Type[] types;
        public static Dictionary<Type, BeanConfig> IOC = new();
        private static Dictionary<Type, List<Type>> DependencyGraph = new();

        private static bool isIOCInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        // Autumn 生命周期
        //  启动 Play
        //        ↓
        // Unity 原生 Awake消息   ← 可提前PushBean
        //        ↓
        //Unity 原生 OnEnable消息
        //        ↓
        static void AutumnUniqueEntry()
        {

            isIOCInitialized = false;
            //    ↓
            InitializeIOC();    //   →  插件 Setup 钩子   →   Unity 原生Awake消息
            //    ↓
            isIOCInitialized = true;   // 此时所有Bean已生成，等待装配
            //    ↓         
            //ScanDependency();
            //    ↓         
            Autowired();      // 自动装配 → 插件 Filter 钩子
            //    ↓
            Call("OnEnable");    // Autumn OnEnable 消息
            //
            Call("Start");     //  Autumn Start 消息
            //    ↓
            CheckEmptywired();  // 空装配检查
            //     ↓    
            Tutorial();
        }



        //     ↓
        // Unity 原生Start消息


        // 后续操作生命周期
        // PushBean() → Autumn Start 消息  →  Autowired()
        // Autowired() → Autumn Filter 消息

        static void Tutorial(){
            Debug.Log(autumnConfig.HelloText);
            if(autumnConfig.FirstStart){
                Debug.Log(autumnConfig.FirstStartMessage);
                autumnConfig.FirstStart=false;
            }
        }
        private static void ScanDependency()
        {
            IEnumerable<FieldInfo> fieldInfos = types.SelectMany(type => type.GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)).Where(fieldInfo => fieldInfo.GetCustomAttribute<Autowired>() != null);
            //TODO
        }

        public static void CheckEmptywired()
        {
            foreach (var fieldInfo in GetAttributedFieldsInfo<Autowired>(types))
            {
                if (fieldInfo.IsStatic)
                {
                    if (check(null))
                        Debug.LogWarning($"{fieldInfo.DeclaringType.FullName} . {fieldInfo} ← 装配为空或空数组或空列表");
                }
                else
                    foreach (var bean in GetBeans(fieldInfo.DeclaringType))
                    {
                        if (check(bean))
                        {
                            Debug.LogWarning($"{fieldInfo.DeclaringType.FullName} . {fieldInfo} ← 装配为空或空数组或空列表");
                        }
                    }
                bool check(object instance)
                {
                    return fieldInfo.GetValue(instance) == null || AutumnUtil.IsEmptyListOrZeroArray(fieldInfo.GetValue(instance));
                }
            }
        }
        private static void InitializeIOC()
        {
            #region 添加Bean操作
            types = Assembly.GetExecutingAssembly().GetTypes();
            BeanTypes = types.Where(t => t.GetCustomAttribute<Bean>() != null).ToArray();
            foreach (Type beanType in BeanTypes)
            {
                if (beanType.IsAbstract && beanType.IsSealed)
                {
                    throw new AutumnCoreException("静态类不能被设置为Bean，因为这是多余的。");
                }
                if (IOC.ContainsKey(beanType))
                {
                }
                else
                {
                    SetupBean(beanType);
                }

            }
            #endregion
        }

        private static BeanConfig SetupBean(Type beanType)
        {
            if (IOC.ContainsKey(beanType))
            {
                throw new AutumnCoreException(autumnConfig.重复安装Bean);
            }

            BeanConfig beanConfig = CreateEmptyBeanConfig(beanType);

            if (beanType.GetCustomAttribute<Bean>().AutoInstantial)
            {
                NewBean(beanType);
            }

            PlugIn(beanType, "Setup", IEnumerableValue =>
            {
                PushExistedBean(beanType, (IEnumerable)IEnumerableValue);
            });
            return beanConfig;
        }

        private static void PlugIn(Type beanType, String methodName, Action<object> operation, params object[] paraments)
        {
            Type[] plugins = beanType.GetCustomAttribute<Bean>().plugins ?? new Type[] { };
            plugins = plugins.Concat(new Type[] { beanType }).ToArray();
            foreach (var pluginType in plugins)
            {
                if (InvokePluginMethod(pluginType, methodName, out object returnValue, paraments))
                {
                    operation?.Invoke(returnValue);
                }
            }
            bool InvokePluginMethod(Type pluginType, String methodName, out object returnValue, params object[] parameters)
            {
                if (typeof(Plugin).IsAssignableFrom(pluginType))
                {
                    MethodInfo methodInfo = pluginType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (methodInfo == null)
                    {
                        returnValue = null;
                        return false;
                    }
                    else
                    {
                        Plugin pluginInstance = (Plugin)Activator.CreateInstance(pluginType);
                        //注入beanType
                        typeof(Plugin).GetField("beanType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(pluginInstance, beanType);

                        returnValue = methodInfo.Invoke(pluginInstance, paraments);
                    }

                }
                else
                {
                    MethodInfo methodInfo = pluginType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (methodInfo == null)
                    {
                        returnValue = null;
                        return false;
                    }
                    returnValue = methodInfo.Invoke(null, paraments);
                }
                return true;
            }
        }
        #region 自动装配
        public static void Autowired(this object any)
        {
            Autowired(new Type[] { any.GetType() });
        }
        public static void Autowired<TClass>()
        {
            Autowired(new Type[] { typeof(TClass) });
        }
        public static void Autowired()
        {
            Autowired(types);
        }
        public static void Autowired(Type[] wiredTypes)
        {

            //装配静态与非静态Bean
            foreach (FieldInfo fieldInfo in GetAttributedFieldsInfo<Autowired>(wiredTypes))
            {
                Autowired autowired = fieldInfo.GetCustomAttribute<Autowired>();

                Type beanType;
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
                IEnumerable<object> Filter(IEnumerable originBeans)
                {
                    foreach (var bean in originBeans)
                    {
                        bool check = true;
                        PlugIn(beanType, "Filter", boolValue =>
                        {
                            if (!(bool)boolValue)
                                check = false;
                        }, bean, autowired.msg);
                        if (check)
                            yield return bean;

                    }
                }

                void Assign(object value)
                {
                    if (fieldInfo.IsStatic)
                        fieldInfo.SetValue(null, value);
                    else
                        foreach (var obj in GetBeans(fieldInfo.DeclaringType))
                            if (obj != null)
                                fieldInfo.SetValue(obj, value);
                            else
                                throw new AutumnCoreException($"{fieldInfo.DeclaringType.FullName} . {fieldInfo} 装配失败,因为{obj}为空");
                }

            }

        }

        #endregion

        #region 增删改查
        public static TBean NewBean<TBean>() where TBean : class
        {
            return NewBean(typeof(TBean)) as TBean;
        }
        public static object NewBean(Type beanType)
        {
            object chain = null;

            switch (BeanConfig.GetEntity(beanType))
            {
                case BeanConfig.Entity.Monobehaviour:
                    GameObject g = new GameObject(beanType.Name);
                    MonoBehaviour.DontDestroyOnLoad(g);

                    chain = g.AddComponent(beanType);
                    break;
                case BeanConfig.Entity.ScriptalObject:

                    chain = ScriptableObject.CreateInstance(beanType); //TODO start方法还没调用
                    break;

                default:
                    var instance = Activator.CreateInstance(beanType);
                    chain = instance;
                    if (isIOCInitialized)
                        instance.GetType().GetMethod("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(instance, null);
                    break;
            }

            PushExistedBean(chain.GetType(), chain);
            if (isIOCInitialized)
            {
                Autowired();
            }
            return chain;
        }
        public static object UnBean(this object any)
        {
            PullExistedBeanForAllBaseType(any);
            if (isIOCInitialized)
            {
                Autowired();
            }
            return any;
        }
        public static object Bean(this object any)
        {
            PushExistedBeanForAllBaseType(any);
            if (isIOCInitialized)
            {
                Autowired();
            }
            return any;
        }
        private static void PullExistedBeanForAllBaseType(object existedBean)
        {
            Type type = existedBean.GetType();
            while (type != null && type.GetCustomAttribute<Bean>() != null)
            {
                if (IOC.TryGetValue(type, out BeanConfig beanConfig))
                {
                    beanConfig.Beans.Remove(existedBean);    //TODO 单独封装为一个函数
                }
                type = type.BaseType;
            }

        }
        private static void PushExistedBeanForAllBaseType(object existedBean)
        {
            Type type = existedBean.GetType();
            while (type != null && type.GetCustomAttribute<Bean>() != null)
            {
                PushExistedBean(type, existedBean);
                type = type.BaseType;
            }
        }
        private static object PushExistedBean(Type beanType, object existedBean)
        {
            if (IOC.TryGetValue(beanType, out BeanConfig beanConfig))
            {
            }
            else
            {
                beanConfig = SetupBean(beanType);
            }
            beanConfig.Beans.Add(existedBean);
            if (isIOCInitialized)
            {
                Autowired();
            }
            return existedBean;
        }
        private static void PushExistedBean(Type beanType, IEnumerable existedBean)
        {
            foreach (var bean in existedBean)
            {
                if (bean != null)
                    PushExistedBean(beanType, bean);
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
        #endregion

        public static void Call(String functionName, params object[] paraments)
        {
            foreach (KeyValuePair<Type, BeanConfig> kvp in IOC)
            {
                switch (kvp.Value.BeanEntity)
                {
                    case BeanConfig.Entity.Monobehaviour:
                        //Autumn消息不支持MonoBehaviour
                        break;

                    default:
                        if (kvp.Value.Beans != null)
                            foreach (var bean in kvp.Value.Beans)
                            {
                                kvp.Key.GetMethod(functionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(bean, paraments);
                            }
                        break;
                }
            }
        }
        private static IEnumerable<FieldInfo> GetAttributedFieldsInfo<TAttribute>(Type[] _types) where TAttribute : Attribute
        {
            return _types.SelectMany(type => type.GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)).Where(field => field.GetCustomAttribute<TAttribute>() != null);
        }
        public static T Harvest<T>() where T : class
        {
            return Harvest(typeof(T)) as T;
        }
        public static object Harvest(Type type)
        {
            return GetBeans(type).First();
        }

        public static List<object> GetBeans(Type type)
        {
            if (type.IsAbstract && type.IsSealed)
            {
                throw new AutumnCoreException($"{type}是一个静态类，请直接调用，而不是使用Autumn");
            }
            else if (IOC.TryGetValue(type, out BeanConfig instance))
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