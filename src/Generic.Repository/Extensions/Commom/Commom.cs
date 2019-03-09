using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Generic.Repository.Extensions.Attributes;

namespace Generic.Repository.Extensions.Commom
{
    public static class Commom
    {
        public static Dictionary<string, IEnumerable<CustomAttributeNamedArgument>> cacheAttribute = new Dictionary<string, IEnumerable<CustomAttributeNamedArgument>>();
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
                    if (saveAttribute)SaveOnCacheAttrIfNonExist(property);
                }
            }
        }

        private static void SaveOnCacheAttrIfNonExist(PropertyInfo propertyInfo)
        {
            cacheAttribute.Add(propertyInfo.Name, propertyInfo.GetCustomAttributesData().FirstOrDefault().NamedArguments);
        }
    }
}
