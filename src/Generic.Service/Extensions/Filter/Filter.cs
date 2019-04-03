using Generic.Service.Enum;
using Generic.Service.Models.BaseModel.BaseFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Generic.Service.Extensions.Filter
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
        /// <typeparam name="TValue">Type Entity</typeparam>
        /// <typeparam name="F">Type Filter</typeparam>
        /// <returns>Predicate generated</returns>
        public static Func<TValue, bool> GenerateLambda<TValue, TFilter>(this TFilter filter)
        where TValue : class
        where TFilter : class, IBaseFilter
        {
            string typeNameTFilter = typeof(TFilter).Name;
            string typeNameTValue = typeof(TValue).Name;

            ParameterExpression param = Expression.Parameter(typeof(TValue));
            Expression<Func<TValue, bool>> predicate = null;
            LambdaMerge mergeOption = LambdaMerge.And;
            LambdaMethod methodOption;

            Commom.Commom.SaveOnCacheIfNonExists<TFilter>(true, true, false, false);
            Commom.Commom.CacheGet[typeNameTFilter].ToList().ForEach(propertyTFilter =>
            {
                Expression lambda = null;
                string namePropertyOnE = null;
                string namePropertyOnTFilter = propertyTFilter.Key;
                var propertyValueTFilter = propertyTFilter.Value(filter);
                if (propertyValueTFilter != null && (!propertyValueTFilter.ToString().Equals("0") || (propertyValueTFilter.GetType() == typeof(DateTime) &&
                        ((DateTime)propertyValueTFilter != DateTime.MinValue || (DateTime)propertyValueTFilter != DateTime.MaxValue))))
                {
                    if (Commom.Commom.CacheAttribute.TryGetValue(typeNameTFilter, out Dictionary<string, Dictionary<string, CustomAttributeTypedArgument>> customAttributes))
                        if (customAttributes.TryGetValue(namePropertyOnTFilter, out Dictionary<string, CustomAttributeTypedArgument> attributes))
                        {
                            if (attributes.TryGetValue("EntityPropertyName", out CustomAttributeTypedArgument attribute))
                            {
                                namePropertyOnE = attribute.Value.ToString();
                            }
                            if (Commom.Commom.CacheProperties[typeNameTValue].TryGetValue(namePropertyOnE ?? propertyTFilter.Key.ToString(), out PropertyInfo property))
                            {
                                if (attributes.TryGetValue("MethodOption", out attribute))
                                {
                                    methodOption = (LambdaMethod)attribute.Value;
                                    lambda = methodOption.SetExpressionType(param, property, propertyValueTFilter);
                                }
                                if (lambda == null)
                                {
                                    throw new ArgumentNullException($"ERROR> ClassName: {nameof(GenerateLambda)} {Environment.NewLine}Message: Predicate is null. Please verify parameters.");
                                }
                                predicate = predicate == null ? lambda.MergeExpressions<TValue>(param) :
                                    predicate.MergeExpressions(mergeOption, param, lambda.MergeExpressions<TValue>(param));
                                if (attributes.TryGetValue("MergeOption", out attribute))
                                {
                                    mergeOption = (LambdaMerge)attribute.Value;
                                }
                                else
                                {
                                    mergeOption = LambdaMerge.And;
                                }
                            }
                        }
                }
            });
            return predicate.Compile();
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
                default: return Expression.Equal(Expression.Property(parameter, prop), Expression.Constant(value));
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

        private static Expression<Func<TValue, bool>> MergeExpressions<TValue>(this Expression lambda, ParameterExpression parameter)
        where TValue : class => Expression.Lambda<Func<TValue, bool>>(lambda, parameter);

        private static Expression<Func<TValue, bool>> MergeExpressions<TValue>(this Expression<Func<TValue, bool>> predicate, LambdaMerge typeMerge, ParameterExpression parameter, Expression<Func<TValue, bool>> predicateMerge)
        where TValue : class
        {
            Expression lambda = null;
            if (typeMerge == LambdaMerge.And)
            {
                lambda = Expression.AndAlso(
                    Expression.Invoke(predicate, parameter),
                    Expression.Invoke(predicateMerge, parameter));
            }
            else
            {
                lambda = Expression.OrElse(
                Expression.Invoke(predicate, parameter),
                Expression.Invoke(predicateMerge, parameter));
            }
            return Expression.Lambda<Func<TValue, bool>>(lambda, parameter);
        }
    }
}