using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Generic.Repository.Extensions.Attributes;

namespace Generic.Repository.Extensions.Commom
{
    public static class Commom
    {
        internal static int totalTypesInAssemblyModel { get; set; }
        public static Dictionary<string, Dictionary<string, Dictionary<string, CustomAttributeTypedArgument>>> CacheAttribute { get; private set; }
        public static Dictionary<string, Dictionary<string, PropertyInfo>> Cache { get; private set;}
        public static Dictionary<Type, MethodInfo> CacheMethod { get; } = new Dictionary<Type, MethodInfo>();

        public static void SetNamespace(string assemblyName, string nameSpace)
        {
            totalTypesInAssemblyModel = Assembly.Load(assemblyName).GetTypes().Where(x => x.Namespace == nameSpace).Count();
            Cache = new Dictionary<string, Dictionary<string, PropertyInfo>>(totalTypesInAssemblyModel);
            CacheAttribute = new Dictionary<string, Dictionary<string, Dictionary<string, CustomAttributeTypedArgument>>>(totalTypesInAssemblyModel);
        }
        public static void SaveOnCacheIfNonExists(Type type, bool saveAttribute = false)
        {
            string typeName = type.Name;
            var propertiesList = type.GetProperties();
            if (!Cache.TryGetValue(typeName, out Dictionary<string, PropertyInfo> dicProperties))
            {
                foreach (var property in propertiesList)
                {
                    if (Cache.ContainsKey(typeName))
                    {
                        Cache[typeName].Add(property.Name, property);
                    }
                    else
                    {
                        Cache.Add(typeName, new Dictionary<string, PropertyInfo>() { { property.Name, property } });
                    }
                    if (saveAttribute) SaveOnCacheAttrIfNonExist(property, typeName, propertiesList.Count());
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
    }
}
