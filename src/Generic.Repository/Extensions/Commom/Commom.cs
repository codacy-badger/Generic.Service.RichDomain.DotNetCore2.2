using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Generic.Repository.Extensions.Attributes;

namespace Generic.Repository.Extensions.Commom
{
    public static class Commom
    {
        public static Dictionary<string, Dictionary<string, CustomAttributeTypedArgument>> cacheAttribute = new Dictionary<string, Dictionary<string, CustomAttributeTypedArgument>>();
        public static Dictionary<string, Dictionary<string, PropertyInfo>> cache { get; } = new Dictionary<string, Dictionary<string, PropertyInfo>>();
        public static Dictionary<Type, MethodInfo> cacheMethod { get; } = new Dictionary<Type, MethodInfo>();

        public static void SaveOnCacheIfNonExists(Type type, bool saveAttribute = false)
        {
            string typeName = type.Name;
            if (!cache.TryGetValue(typeName, out Dictionary<string, PropertyInfo> dicProperties))
            {
                foreach (var property in type.GetProperties())
                {
                    if (cache.ContainsKey(typeName))
                    {
                        cache[typeName].Add(property.Name, property);
                    }
                    else
                    {
                        cache.Add(typeName, new Dictionary<string, PropertyInfo>() { { property.Name, property } });
                    }
                    if (saveAttribute) SaveOnCacheAttrIfNonExist(property);
                }
            }
        }

        private static void SaveOnCacheAttrIfNonExist(PropertyInfo propertyInfo)
        {
            propertyInfo.GetCustomAttributesData().FirstOrDefault().NamedArguments.ToList().ForEach(attr =>
            {
                if (!cacheAttribute.ContainsKey(propertyInfo.Name))
                    cacheAttribute.Add(propertyInfo.Name, new Dictionary<string, CustomAttributeTypedArgument>() { { attr.MemberName, attr.TypedValue } });
                else cacheAttribute[propertyInfo.Name].Add(attr.MemberName, attr.TypedValue);
            });
        }

        public static void VerifyAndSaveCache(this Type type, bool saveAttribute = false)
        {
            if (!cache.ContainsKey(type.Name))
                SaveOnCacheIfNonExists(type, saveAttribute);
        }
    }
}
