using System;
using System.Collections.Generic;
using System.Reflection;

namespace Generic.Repository.Extensions.Commom
{
    public static class Commom
    {
        public static Dictionary<Type, Dictionary<string, PropertyInfo>> cache { get; } = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        public static Dictionary<Type, MethodInfo> cacheMethod { get; } = new Dictionary<Type, MethodInfo>();
        
        public static void SaveOnCacheIfNonExists(Type type)
        {
            if (!Commom.cache.TryGetValue(type, out Dictionary<string, PropertyInfo> dicProperties))
            {
                foreach (var property in type.GetProperties())
                {
                    if (Commom.cache.ContainsKey(type))
                    {
                        Commom.cache[type].Add(property.Name, property);
                    }
                    else
                    {
                        Commom.cache.Add(type, new Dictionary<string, PropertyInfo>() { { property.Name, property } });
                    }
                }
            }
        }
    }
}
