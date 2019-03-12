using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Generic.Repository.Enum;
using Generic.Repository.Extensions.Attributes;
using Generic.Repository.Extensions.Commom;
using Generic.Repository.Models.BaseEntity.BaseFilter;

namespace Generic.Repository.Extensions.Filter
{
    /// <summary>
    /// Extension Filter to generate lambda.
    /// </summary>
    public static class Filter
    {
        /// <summary>
        /// Generate lambda method
        /// </summary>
        /// <param name="filter">Object filter</param>
        /// <typeparam name="E">Type Entity</typeparam>
        /// <typeparam name="F">Type Filter</typeparam>
        /// <returns>Predicate generated</returns>
        public static Expression<Func<E, bool>> GenerateLambda<E, F>(this F filter)
        where E : class
        where F : IBaseFilter
        {
            var typeF = typeof(F);
            var typeE = typeof(E);

            Dictionary<string, CustomAttributeTypedArgument> attributes;
            ParameterExpression param = Expression.Parameter(typeE);
            Expression<Func<E, bool>> predicate = null;
            LambdaMerge mergeOption = LambdaMerge.And;
            CustomAttributeTypedArgument attribute;
            LambdaMethod methodOption;
            PropertyInfo paramProp;

            string typeEName = typeE.Name;
            Commom.Commom.SaveOnCacheIfNonExists<E>();
            Commom.Commom.SaveOnCacheIfNonExists<E>(true);

            if (Commom.Commom.Cache.TryGetValue(typeF.Name, out Dictionary<string, PropertyInfo> dicPropertiesF))
            {
                dicPropertiesF.Values.ToList().ForEach(prop =>
                {
                    Expression lambda = null;
                    object nameProperty = null;
                    string propName = prop.Name;
                    var propValue = prop.GetValue(filter, null);

                    if (propValue != null && !propValue.ToString().Equals("0") || (prop.PropertyType == typeof(DateTime)
                        && ((DateTime)propValue != DateTime.MinValue || (DateTime)propValue != DateTime.MaxValue)))
                    {
                        if (Commom.Commom.CacheAttribute.TryGetValue(typeF.Name, out Dictionary<string, Dictionary<string, CustomAttributeTypedArgument>> properties))
                            if (properties.TryGetValue(propName, out attributes))
                            {
                                if (attributes.TryGetValue("EntityPropertyName", out attribute))
                                    nameProperty = attribute.Value;

                                if (Commom.Commom.Cache.TryGetValue(typeEName, out Dictionary<string, PropertyInfo> dicPropertiesE))
                                    if (dicPropertiesE.TryGetValue(nameProperty != null ? nameProperty.ToString() : propName, out paramProp))
                                        if (attributes.TryGetValue("MethodOption", out attribute))
                                        {
                                            methodOption = (LambdaMethod)attribute.Value;
                                            lambda = methodOption.SetExpressionType(param, paramProp, propValue);
                                        }
                                if (lambda == null)
                                    throw new ArgumentNullException($"ERROR> ClassName: {nameof(GenerateLambda)} {Environment.NewLine}Message: Lambda is null.");
                                predicate = predicate == null ? lambda.MergeExpressions<E>(param)
                                    : predicate.MergeExpressions<E>(mergeOption, param, lambda.MergeExpressions<E>(param));

                                if (attributes.TryGetValue("MergeOption", out attribute))
                                    mergeOption = (LambdaMerge)attribute.Value;
                                else mergeOption = LambdaMerge.And;
                            }
                    }
                });
            }
            else
            {
                throw new ArgumentNullException($"ERROR> ClassName: {nameof(Commom.Commom.SaveOnCacheIfNonExists)} {Environment.NewLine}Message: Cache is null.");
            }
            return predicate;
        }
        /// <summary>
        /// Create an expression
        /// </summary>
        /// <param name="type">Type expression to create</param>
        /// <param name="parameter">Parameter to will be used to make an expression</param>
        /// <param name="prop">Property to will be used to make an expression</param>
        /// <param name="value">Value to will be used to make an expression</param>
        /// <returns></returns>
        private static Expression SetExpressionType(this LambdaMethod type, ParameterExpression parameter, PropertyInfo prop, object value)
        {
            Expression lambda = null;
            switch (type)
            {
                case LambdaMethod.Equals:
                    return Expression.Equal(Expression.Property(parameter, prop), Expression.Constant(value));
                case LambdaMethod.Contains:
                    if (prop.PropertyType != typeof(string))
                        throw new NotSupportedException($"ERROR> ClassName: {nameof(SetExpressionType)} {Environment.NewLine}Message: {prop.Name} type is not string. This method only can be used by string type parameter.");
                    MethodInfo method = typeof(string).GetMethod(LambdaMethod.Contains.ToString(), new[] { typeof(string) });
                    lambda = Expression.Call(Expression.Property(parameter, prop), method, Expression.Constant(value));
                    break;
                case LambdaMethod.GreaterThan:
                    if (prop.IsNotString(LambdaMethod.GreaterThan.ToString()))
                        lambda = Expression.GreaterThan(Expression.Property(parameter, prop), Expression.Constant(value));
                    break;
                case LambdaMethod.LessThan:
                    if (prop.IsNotString(LambdaMethod.LessThan.ToString()))
                        lambda = Expression.LessThan(Expression.Property(parameter, prop), Expression.Constant(value));
                    break;
                case LambdaMethod.GreaterThanOrEqual:
                    if (prop.IsNotString(LambdaMethod.GreaterThanOrEqual.ToString()))
                        lambda = Expression.GreaterThanOrEqual(Expression.Property(parameter, prop), Expression.Constant(value));
                    break;
                case LambdaMethod.LessThanOrEqual:
                    if (prop.IsNotString(LambdaMethod.GreaterThanOrEqual.ToString()))
                        lambda = Expression.LessThanOrEqual(Expression.Property(parameter, prop), Expression.Constant(value));
                    break;
            }
            return lambda;
        }

        /// <summary>
        /// Validation data
        /// </summary>
        /// <param name="prop">Property to validate</param>
        /// <param name="typeMethod">Name of method will be used</param>
        /// <returns>Return a bool</returns>
        private static bool IsNotString(this PropertyInfo prop, string typeMethod)
        {
            if (prop.PropertyType != typeof(String))
                return true;
            else throw new NotSupportedException($"ERROR> ClassName: {nameof(SetExpressionType)} {Environment.NewLine}Message: {prop.Name} type is string. {typeMethod} method doesn't support this type. Please inform Contains or Equal.");
        }

        private static Expression<Func<E, bool>> MergeExpressions<E>(this Expression lambda, ParameterExpression parameter)
        where E : class => Expression.Lambda<Func<E, bool>>(lambda, parameter);

        private static Expression<Func<E, bool>> MergeExpressions<E>(this Expression<Func<E, bool>> predicate, LambdaMerge typeMerge, ParameterExpression parameter, Expression<Func<E, bool>> predicateMerge)
        where E : class
        {
            Expression lambda = null;
            switch (typeMerge)
            {
                case LambdaMerge.And:
                    lambda = Expression.AndAlso(
                        Expression.Invoke(predicate, parameter),
                        Expression.Invoke(predicateMerge, parameter));
                    break;
                case LambdaMerge.Or:
                    lambda = Expression.OrElse(
                        Expression.Invoke(predicate, parameter),
                        Expression.Invoke(predicateMerge, parameter));
                    break;
            }
            return Expression.Lambda<Func<E, bool>>(lambda, parameter);
        }

        private static void MapFilter<F>(this F filter)
        where F : IBaseFilter
        {

        }
    }
}