using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Generic.Repository.Entity.IFilter;
using Generic.Repository.Enum;
using Generic.Repository.Extension.Attributes;

namespace Generic.Repository.Extension.Repository
{
    public static class Repository
    {
        public static Expression<Func<E, bool>> GenerateLambda<E, F>(this F filter)
        where E : class
        where F : IBaseFilter
        {

            ParameterExpression param = Expression.Parameter(typeof(E));
            Expression<Func<E, bool>> predicate = null;
            LambdaMerge mergeOption = LambdaMerge.And;
            LambdaMethod methodOption;
            PropertyInfo paramProp;
            filter.GetType().GetProperties().ToList().ForEach(prop =>
            {
                var propValue = prop.GetValue(filter, null);
                if (propValue != null && !propValue.ToString().Equals("0"))
                {
                    prop.GetCustomAttributes(typeof(LambdaAttribute), true).ToList().ForEach(attr =>
                    {
                        var nameProperty = attr.GetType().GetProperties().FirstOrDefault(x => x.Name == "EntityPropertyName").GetValue(attr, null);
                        Expression lambda = null;

                        if (nameProperty != null)
                            paramProp = typeof(E).GetProperty(nameProperty.ToString());
                        else
                            paramProp = typeof(E).GetProperty(prop.Name);

                        methodOption = (LambdaMethod)attr.GetType().GetProperty("MethodOption").GetValue(attr, null);
                        lambda = methodOption.SetExpressionType(param, paramProp, propValue);
                        if (predicate == null)
                            predicate = lambda.MergeExpressions<E>(param);
                        else
                            predicate = predicate.MergeExpressions<E>(mergeOption, param, lambda.MergeExpressions<E>(param));
                        mergeOption = (LambdaMerge)attr.GetType().GetProperty("MergeOption").GetValue(attr, null);

                    });
                }
            });
            return predicate;
        }

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
    }
}