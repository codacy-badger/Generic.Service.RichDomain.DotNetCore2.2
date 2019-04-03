using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Generic.Service.Extensions.Commom
{
    public static class Commom
    {
        internal static bool isExecuted { get; set; } = false;
        internal static int TotalTypesInAssemblyModel { get; set; }
        public static Dictionary<string, Dictionary<string, Func<object, object>>> CacheGet { get; private set; }
        public static Dictionary<string, Dictionary<string, Action<object, object>>> CacheSet { get; private set; }
        public static Dictionary<string, Dictionary<string, Dictionary<string, CustomAttributeTypedArgument>>> CacheAttribute { get; private set; }
        public static Dictionary<string, Dictionary<string, PropertyInfo>> CacheProperties { get; private set; }
        public static Dictionary<Type, MethodInfo> CacheMethod { get; } = new Dictionary<Type, MethodInfo>();

        /// <summary>
        /// Set space on dictionary to improve perfomacing
        /// </summary>
        /// <param name="AssemblyName">Assembly name of project which models alread exist</param>
        /// <param name="Namespace">Namespace name of models alread exist</param>
        public static void SetSizeByLengthProperties(string AssemblyName, string Namespace)
        {
            isExecuted = true;
            AssemblyName.IsNull(nameof(AssemblyName), nameof(SetSizeByLengthProperties));
            Namespace.IsNull(nameof(Namespace), nameof(SetSizeByLengthProperties));
            int size = Assembly.Load(AssemblyName).GetTypes().Where(x => Namespace.Split(";").Contains(x.Namespace)).Count();
            if (size > 0)
            {
                TotalTypesInAssemblyModel = size;
                CacheProperties = new Dictionary<string, Dictionary<string, PropertyInfo>>(TotalTypesInAssemblyModel);
                CacheGet = new Dictionary<string, Dictionary<string, Func<object, object>>>(TotalTypesInAssemblyModel);
                CacheSet = new Dictionary<string, Dictionary<string, Action<object, object>>>(TotalTypesInAssemblyModel);
                CacheAttribute = new Dictionary<string, Dictionary<string, Dictionary<string, CustomAttributeTypedArgument>>>(TotalTypesInAssemblyModel);
            }
            else
            {
                CacheProperties = new Dictionary<string, Dictionary<string, PropertyInfo>>();
                CacheGet = new Dictionary<string, Dictionary<string, Func<object, object>>>();
                CacheSet = new Dictionary<string, Dictionary<string, Action<object, object>>>();
                CacheAttribute = new Dictionary<string, Dictionary<string, Dictionary<string, CustomAttributeTypedArgument>>>();
            }
        }

        public static void SaveOnCacheIfNonExists<TValue>()
        where TValue : class
        {
            SaveOnCacheIfNonExists<TValue>(true, true, true, true);
        }

        public static void SaveOnCacheIfNonExists<TValue>(bool saveAttribute)
        where TValue : class
        {
            SaveOnCacheIfNonExists<TValue>(saveAttribute, true, true, true);
        }

        public static void SaveOnCacheIfNonExists<TValue>(bool saveAttribute, bool saveGet)
        where TValue : class
        {
            SaveOnCacheIfNonExists<TValue>(saveAttribute, saveGet, true, true);
        }

        public static void SaveOnCacheIfNonExists<TValue>(bool saveAttribute, bool saveGet, bool saveSet)
        where TValue : class
        {
            SaveOnCacheIfNonExists<TValue>(saveAttribute, saveGet, saveSet, true);
        }

        public static void SaveOnCacheIfNonExists<TValue>(bool saveAttribute, bool saveGet, bool saveSet, bool saveProperties)
        where TValue : class
        {
            if (!isExecuted)
            {
                throw new OperationCanceledException($"Method {nameof(SetSizeByLengthProperties)} is not instantiate. Please invoke this method!");
            }
            string typeName = typeof(TValue).Name;
            PropertyInfo[] properties = null;

            if (!CacheGet.ContainsKey(typeName) || !CacheSet.ContainsKey(typeName) || !CacheProperties.ContainsKey(typeName))
            {
                properties = typeof(TValue).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                int totalProperties = properties.Length;

                if (saveGet && !CacheGet.ContainsKey(typeName))
                {
                    CacheGet.Add(typeName, properties.ToDictionary(g => g.Name, m => CreateGetter<TValue>(m)));
                }
                if (saveSet && !CacheSet.ContainsKey(typeName))
                {
                    CacheSet.Add(typeName, properties.ToDictionary(s => s.Name, m => CreateSetter<TValue>(m)));
                }
                if (saveProperties && !CacheProperties.ContainsKey(typeName))
                {
                    CacheProperties.Add(typeName, properties.ToDictionary(p => p.Name, p => p));
                }
                if (saveAttribute && !CacheAttribute.ContainsKey(typeName))
                {
                    CacheAttribute.Add(typeName, new Dictionary<string, Dictionary<string, CustomAttributeTypedArgument>>(totalProperties));
                    foreach (var property in properties)
                        SaveOnCacheAttrIfNonExist(property, typeName, totalProperties);
                }
            }
        }

        private static void SaveOnCacheAttrIfNonExist(PropertyInfo propertyInfo, string typeName, int totalProperties)
        {
            string propetyName = propertyInfo.Name;
            CacheAttribute[typeName].Add(propetyName, propertyInfo.GetCustomAttributesData().SelectMany(x => x.NamedArguments).ToDictionary(x => x.MemberName, x => x.TypedValue));
        }

        private static Func<object, object> CreateGetter<TValue>(PropertyInfo property)
        {
            property.IsNull(property.Name, nameof(CreateGetter));
            var getter = property.GetGetMethod(true);
            if (getter == null)
            {
                throw new ArgumentException($"ERROR> ClassName: {nameof(CreateGetter)} {Environment.NewLine}Message: The property {property.Name} does not have a public accessor.");
            }
            MethodInfo genericMethod = typeof(Commom).GetMethod("CreateGetterGeneric", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo genericHelper = genericMethod.MakeGenericMethod(typeof(TValue), property.PropertyType);
            return (Func<object, object>)genericHelper.Invoke(null, new object[] { getter });
        }

        private static Func<object, object> CreateGetterGeneric<TValue, TReturn>(MethodInfo getter) where TValue : class
        {
            Func<TValue, TReturn> getterTypedDelegate = (Func<TValue, TReturn>)Delegate.CreateDelegate(typeof(Func<TValue, TReturn>), getter);
            Func<object, object> getterDelegate = ((object instance) => getterTypedDelegate((TValue)instance));
            return getterDelegate;
        }

        private static Action<object, object> CreateSetter<TValue>(PropertyInfo property)
        {
            property.IsNull(property.Name, nameof(CreateSetter));
            
            var setter = property.GetSetMethod(true);
            if (setter == null)
            {
                throw new ArgumentException($"ERROR> ClassName: {nameof(CreateSetter)} {Environment.NewLine}Message: The property {property.Name} does not have a public setter.");
            }
            var genericMethod = typeof(Commom).GetMethod("CreateSetterGeneric", BindingFlags.NonPublic | BindingFlags.Static);

            MethodInfo genericHelper = genericMethod.MakeGenericMethod(typeof(TValue), property.PropertyType);
            return (Action<object, object>)genericHelper.Invoke(null, new object[] { setter });
        }

        private static Action<object, object> CreateSetterGeneric<TValue, TInput>(MethodInfo setter) where TValue : class
        {
            Action<TValue, TInput> setterTypedDelegate = (Action<TValue, TInput>)Delegate.CreateDelegate(typeof(Action<TValue, TInput>), setter);
            Action<object, object> setterDelegate = (object instance, object value) => { setterTypedDelegate((TValue)instance, (TInput)value); };
            return setterDelegate;
        }

        private static void IsNull(this object value, string nameObject, string nameClass)
        {
            if (value.GetType() == typeof(string) && string.IsNullOrEmpty(value.ToString()))
            {
                throw new ArgumentNullException($"ERROR> ClassName: {nameClass} {Environment.NewLine} Message: {nameObject} is null or empty.");
            }
            else if (value == null)
            {
                throw new ArgumentNullException($"ERROR> ClassName: {nameClass} {Environment.NewLine} Message: {nameObject} is null or empty.");
            }
        }
    }
}