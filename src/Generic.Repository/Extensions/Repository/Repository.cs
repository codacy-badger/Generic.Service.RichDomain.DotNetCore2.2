using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Generic.Repository.Base.Filter;

namespace Generic.Repository.Extension.Repository {
    public static class Repository {
        public static Expression<Func<E, bool>> GenerateLambda<E, F> (this F filter)
        where E : class
        where F : BaseFilter {

            ParameterExpression param = Expression.Parameter (typeof (E));
            Expression<Func<E, bool>> predicate = null;
            string MergeExpressionType = "";
            string typeExpression = "";

            filter.GetType ().GetProperties ().ToList ().ForEach (prop => {
                var propValue = prop.GetValue (filter, null);
                if (propValue != null) {
                    Expression lambda = null;
                    typeExpression = Regex.Replace (prop.Name, @"[^(Equal)|(Contains)|(GreaterThan)|(LessThan)|(GreaterThanOrEquals)|(LessThanOrEquals)]", string.Empty);
                    MergeExpressionType = Regex.Replace (prop.Name, @"[^(And)|(Or)]", string.Empty);
                    var paramProp = typeof (E).GetProperty (prop.Name);

                    if (predicate == null) {
                        predicate = lambda.MergeExpressions<E> (param);
                    } else {
                        predicate = predicate.MergeExpressions<E> (MergeExpressionType, param, lambda.MergeExpressions<E> (param));
                    }
                }
            });
            return predicate;
        }

        private static Expression SetExpressionType (this string type, ParameterExpression parameter, PropertyInfo prop, object value) {
            Expression lambda = null;
            switch (type) {
                case "Equals":
                    return Expression.Equal (Expression.Property (parameter, prop), Expression.Constant (value));
                case "Contains":
                    if (value.GetType () == typeof (string)) {
                        MethodInfo method = typeof (string).GetMethod ("Contains", new [] { typeof (string) });
                        lambda = Expression.Call (Expression.Property (parameter, prop), method, Expression.Constant (value));
                    } else
                        throw new Exception ($"NameClass: {nameof(SetExpressionType)} - Type of {prop.Name} is not string");
                    break;
                case "GreaterThan":
                    if (prop.ValidateTypeIsNotString ("GreaterThan"))
                        lambda = Expression.GreaterThan (Expression.Property (parameter, prop), Expression.Constant (value));
                    break;
                case "LessThan":
                    if (prop.ValidateTypeIsNotString ("LessThan"))
                        lambda = Expression.LessThan (Expression.Property (parameter, prop), Expression.Constant (value));
                    break;
                case "GreaterThanOrEqual":
                    if (prop.ValidateTypeIsNotString ("GreaterThanOrEqual"))
                        lambda = Expression.GreaterThanOrEqual (Expression.Property (parameter, prop), Expression.Constant (value));
                    break;
                case "LessThanOrEqual":
                    if (prop.ValidateTypeIsNotString ("LessThanOrEqual"))
                        lambda = Expression.LessThanOrEqual (Expression.Property (parameter, prop), Expression.Constant (value));
                    break;
                default:
                    lambda = Expression.Equal (Expression.Property (parameter, prop), Expression.Constant (value));
                    break;
            }
            return lambda;
        }

        private static bool ValidateTypeIsNotString (this PropertyInfo prop, string typeMethod) {
            if (prop.GetType () != typeof (string))
                return true;
            else throw new Exception ($"NameClass: {nameof(SetExpressionType)} - Type of {prop.Name} is string. Method {typeMethod} doesn't support this.");
        }

        private static Expression<Func<E, bool>> MergeExpressions<E> (this Expression lambda, ParameterExpression parameter)
        where E : class => Expression.Lambda<Func<E, bool>> (lambda, parameter);

        private static Expression<Func<E, bool>> MergeExpressions<E> (this Expression<Func<E, bool>> predicate, string typeMerge, ParameterExpression parameter, Expression<Func<E, bool>> predicateMerge)
        where E : class {
            Expression lambda = null;
            switch (typeMerge) {
                case "And":
                    lambda = Expression.AndAlso (
                        Expression.Invoke (predicate, parameter),
                        Expression.Invoke (predicateMerge, parameter));
                    break;
                case "Or":
                    lambda = Expression.OrElse (
                        Expression.Invoke (predicate, parameter),
                        Expression.Invoke (predicateMerge, parameter));
                    break;
                default:
                    lambda = Expression.AndAlso (
                        Expression.Invoke (predicate, parameter),
                        Expression.Invoke (predicateMerge, parameter));
                    break;
            }
            return Expression.Lambda<Func<E, bool>> (lambda, parameter);
        }

        private static bool ReturnString(){
            
        }
    }
}