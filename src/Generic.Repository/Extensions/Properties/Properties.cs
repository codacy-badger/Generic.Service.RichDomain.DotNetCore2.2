using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Generic.Repository.Extensions.Properties
{
    public static class Properties<E>
    where E : class
    {
        public static Dictionary<string, Dictionary<string, Func<E, object>>> CacheGet { get; private set; }
        public static Dictionary<string, Dictionary<string, Action<E, object>>> CacheSet { get; private set; }

        public static void SetDefaultSizeOnCache()
        {
            CacheGet = CacheGet ?? new Dictionary<string, Dictionary<string, Func<E, object>>>(Commom.Commom.totalTypesInAssemblyModel);
            CacheSet = CacheSet ?? new Dictionary<string, Dictionary<string, Action<E, object>>>(Commom.Commom.totalTypesInAssemblyModel);
        }
    }
}
