using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Generic.Repository.Extensions.Properties
{
    public static class Properties<E>
    where E : class
    {
        public static Dictionary<string, Dictionary<string, Func<E, object>>> CacheGet { get; private set; } = new Dictionary<string, Dictionary<string, Func<E, object>>>();
        public static Dictionary<string, Dictionary<string, Action<E, object>>> CacheSet { get; private set; }= new Dictionary<string, Dictionary<string, Action<E, object>>>();

        public static void SetDefaultSizeOnCache(string assemblyName, string nameSpace)
        {
            Commom.Commom.totalTypesInAssemblyModel = Assembly.Load(assemblyName).GetTypes().Where(x => nameSpace.Split(";").Contains(x.Namespace)).Count();
            CacheGet = new Dictionary<string, Dictionary<string, Func<E, object>>>(Commom.Commom.totalTypesInAssemblyModel);
            CacheSet = new Dictionary<string, Dictionary<string, Action<E, object>>>(Commom.Commom.totalTypesInAssemblyModel);
        }
    }
}
