using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Generic.Repository.Extensions.Attributes;
using Generic.Repository.Extensions.Expressions;
using Generic.Repository.Extensions.Properties;

namespace Generic.Repository.Extensions.Commom
{
    public static class Commom
    {
        internal static int totalTypesInAssemblyModel { get; set; }
        public static Dictionary<string, Dictionary<string, Dictionary<string, CustomAttributeTypedArgument>>> CacheAttribute { get; private set; }
        public static Dictionary<string, Dictionary<string, PropertyInfo>> Cache { get; private set; }
        public static Dictionary<Type, MethodInfo> CacheMethod { get; } = new Dictionary<Type, MethodInfo>();

        /// <summary>
        /// Set space on dictionary to improve perfomacing
        /// </summary>
        /// <param name="assemblyName">Assembly name of project which models alred exist</param>
        /// <param name="nameSpace">Namespace of models and filters, using separator ";" to write differents namespace</param>
        public static void SetNamespace(string assemblyName, string nameSpace)
        {
            Cache = new Dictionary<string, Dictionary<string, PropertyInfo>>(totalTypesInAssemblyModel);
            CacheAttribute = new Dictionary<string, Dictionary<string, Dictionary<string, CustomAttributeTypedArgument>>>(totalTypesInAssemblyModel);
        }
        
        public static void SaveOnCacheIfNonExists<E>(bool saveAttribute = false)
        where E : class
        {
            string typeName = typeof(E).Name;
            PropertyInfo[] properties = typeof(E).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            int totalProperties = properties.Count();
            if (!Properties<E>.CacheGet.ContainsKey(typeName) && !Properties<E>.CacheSet.ContainsKey(typeName))
            {
                Properties<E>.CacheGet.Add(typeName, properties.ToDictionary(g => g.Name, m => CreateGetter<E>(m)));
                Properties<E>.CacheSet.Add(typeName, properties.ToDictionary(s => s.Name, m => CreateSetter<E>(m)));
            }
            if (saveAttribute)
            {
                foreach (var property in properties)
                {
                    SaveOnCacheAttrIfNonExist(property, typeName, totalProperties);
                }
            }
        }

        private static void SaveOnCacheAttrIfNonExist(PropertyInfo propertyInfo, string typeName, int totalProperties)
        {
            string propetyName = propertyInfo.Name;
            if (!CacheAttribute.ContainsKey(typeName))
            {
                CacheAttribute.Add(typeName, new Dictionary<string, Dictionary<string, CustomAttributeTypedArgument>>(totalProperties));
            }
            if (!CacheAttribute[typeName].ContainsKey(propetyName))
            {
                CacheAttribute[typeName].Add(propetyName, new Dictionary<string, CustomAttributeTypedArgument>(3));
            }
            propertyInfo.GetCustomAttributesData().ToList().ForEach(customAttr =>
            {
                customAttr.NamedArguments.ToList().ForEach(attr =>
                 {
                     CacheAttribute[typeName][propetyName].Add(attr.MemberName, attr.TypedValue);
                 });
            });
        }

        private static Func<E, object> CreateGetter<E>(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            var getter = property.GetGetMethod(true);
            if (getter == null)
                throw new ArgumentException($"The property {property.Name} does not have a public accessor.");

            var genericMethod = typeof(Commom).GetMethod("CreateGetterGeneric", BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo genericHelper = genericMethod.MakeGenericMethod(typeof(E), property.PropertyType);
            return (Func<E, object>)genericHelper.Invoke(null, new object[] { getter });
        }

        private static Func<object, object> CreateGetterGeneric<T, R>(MethodInfo getter) where T : class
        {
            Func<T, R> getterTypedDelegate = (Func<T, R>)Delegate.CreateDelegate(typeof(Func<T, R>), getter);
            Func<object, object> getterDelegate = (Func<object, object>)((object instance) => getterTypedDelegate((T)instance));
            return getterDelegate;
        }

        private static Action<E, object> CreateSetter<E>(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            var setter = property.GetSetMethod(true);
            if (setter == null)
                throw new ArgumentException($"The property {property.Name} does not have a public setter.");

            var genericMethod = typeof(Commom).GetMethod("CreateSetterGeneric", BindingFlags.NonPublic | BindingFlags.Static);

            MethodInfo genericHelper = genericMethod.MakeGenericMethod(typeof(E), property.PropertyType);
            return (Action<E, object>)genericHelper.Invoke(null, new object[] { setter });
        }

        private static Action<object, object> CreateSetterGeneric<T, V>(MethodInfo setter) where T : class
        {
            Action<T, V> setterTypedDelegate = (Action<T, V>)Delegate.CreateDelegate(typeof(Action<T, V>), setter);
            Action<object, object> setterDelegate = (Action<object, object>)((object instance, object value) => { setterTypedDelegate((T)instance, (V)value); });
            return setterDelegate;
        }
    }
}
