using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Generic.Repository.Extensions.Expressions
{
    public static class CacheExpressions<E>
    {
        private static Expression<Func<E, E>> expression;
        public static Dictionary<string, Dictionary<string, Expression<Func<E, E>>>> CacheOrdenation { get; } = new Dictionary<string, Dictionary<string, Expression<Func<E, E>>>>();

        private static void SaveOrderExpressionIfNonExists(string nameField)
        {
            string nameType = typeof(E).Name;
            if (!CacheExpressions<E>.CacheOrdenation.ContainsKey(nameType))
            {
                CacheExpressions<E>.CacheOrdenation.Add(nameType, new Dictionary<string, Expression<Func<E, E>>>());
            }
            if (!CacheExpressions<E>.CacheOrdenation[nameType].ContainsKey(nameField))
            {
                if (Commom.Commom.Cache.TryGetValue(nameType, out Dictionary<string, PropertyInfo> properties))
                    if (properties.TryGetValue(nameField, out PropertyInfo property))
                    {
                        Expression<Func<E, E>> t = x => (E)property.GetValue(x, null);
                        CacheExpressions<E>.CacheOrdenation[nameType].Add(nameField, t);
                    }
            }
        }

        public static Expression<Func<E, E>> VerifyExpression(string nameField)
        {
            SaveOrderExpressionIfNonExists(nameField);
            if (CacheOrdenation[typeof(E).Name].TryGetValue(nameField, out expression))
                return expression;
            else throw new NullReferenceException($"ERROR> ClassName: {nameof(SaveOrderExpressionIfNonExists)} {Environment.NewLine}Message: Error to add type {nameof(E)} to dictionary, please verify the code.");
        }

    }
}
