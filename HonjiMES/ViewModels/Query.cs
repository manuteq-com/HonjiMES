using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace HonjiMES.Models
{
    public class Query
    {
        public enum Operators
        {
            /// <summary>
            /// 沒做用
            /// </summary>
            None,
            /// <summary>
            /// =
            /// </summary>
            Equal,
            /// <summary>
            /// !=
            /// </summary>
            NotEqual,
            /// <summary>
            /// 〉
            /// </summary>
            GreaterThan,
            /// <summary>
            /// 〉=
            /// </summary>
            GreaterThanOrEqual,
            /// <summary>
            /// 〈
            /// </summary>
            LessThan,
            /// <summary>
            /// 〈=
            /// </summary>
            LessThanOrEqual,
            /// <summary>
            /// Contains
            /// </summary>
            Contains,
            /// <summary>
            /// NotContains
            /// </summary>
            NotContains,
            /// <summary>
            /// StartWith
            /// </summary>
            StartWith,
            /// <summary>
            /// EndWidth
            /// </summary>
            EndWidth,
            /// <summary>
            /// Range
            /// </summary>
            Range,
        }
        public enum Condition
        {
            OrElse = 1,
            AndAlso = 2
        }
        public string Name { get; set; }
        public Operators Operator { get; set; }
        public object Value { get; set; }
        public object ValueMin { get; set; }
        public object ValueMax { get; set; }
    }

    public class QueryCollection : Collection<Query>
    {
        private static MemberExpression GetMemberExpression(ParameterExpression parameter, string propName)
        {
            if (string.IsNullOrEmpty(propName)) return null;
            var propertiesName = propName.Split('.');
            if (propertiesName.Count() == 2)
                return Expression.Property(Expression.Property(parameter, propertiesName[0]), propertiesName[1]);
            return Expression.Property(parameter, propName);
        }
        public Expression<Func<T, bool>> AsExpression<T>(Query.Condition condition = Query.Condition.AndAlso) where T : class
        {
            Type targetType = typeof(T);
            TypeInfo typeInfo = targetType.GetTypeInfo();
            var parameter = Expression.Parameter(targetType, "x");
            Expression expression = null;
            Func<Expression, Expression, Expression> Append = (exp1, exp2) =>
            {
                if (exp1 == null)
                {
                    return exp2;
                }
                return condition == Query.Condition.OrElse ? Expression.OrElse(exp1, exp2) : Expression.AndAlso(exp1, exp2);
            };
            foreach (var item in this)
            {
                var property = typeInfo.GetProperty(item.Name);
                if (property == null)
                {
                    property = GetLastProperty(item.Name, typeInfo);
                }
                if (property == null ||
                    !property.CanRead ||
                    (item.Operator != Query.Operators.Range && item.Value == null) ||
                    (item.Operator == Query.Operators.Range && item.ValueMin == null && item.ValueMax == null))
                {
                    continue;
                }
                Type realType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                if (item.Value != null)
                {
                    item.Value = Convert.ChangeType(item.Value, realType);
                }
                Expression<Func<object>> valueLamba = () => item.Value;

                try
                {
                    switch (item.Operator)
                    {
                        case Query.Operators.Equal:
                            {
                                expression = Append(expression, Expression.Equal(GetMemberExpression(parameter, item.Name),
                                    Expression.Convert(valueLamba.Body, property.PropertyType)));
                                break;
                            }
                        case Query.Operators.NotEqual:
                            {
                                expression = Append(expression, Expression.NotEqual(GetMemberExpression(parameter, item.Name),
                                    Expression.Convert(valueLamba.Body, property.PropertyType)));
                                break;
                            }
                        case Query.Operators.GreaterThan:
                            {
                                expression = Append(expression, Expression.GreaterThan(GetMemberExpression(parameter, item.Name),
                                    Expression.Convert(valueLamba.Body, property.PropertyType)));
                                break;
                            }
                        case Query.Operators.GreaterThanOrEqual:
                            {
                                expression = Append(expression, Expression.GreaterThanOrEqual(GetMemberExpression(parameter, item.Name),
                                    Expression.Convert(valueLamba.Body, property.PropertyType)));
                                break;
                            }
                        case Query.Operators.LessThan:
                            {
                                expression = Append(expression, Expression.LessThan(GetMemberExpression(parameter, item.Name),
                                    Expression.Convert(valueLamba.Body, property.PropertyType)));
                                break;
                            }
                        case Query.Operators.LessThanOrEqual:
                            {
                                expression = Append(expression, Expression.LessThanOrEqual(GetMemberExpression(parameter, item.Name),
                                    Expression.Convert(valueLamba.Body, property.PropertyType)));
                                break;
                            }
                        case Query.Operators.Contains:
                            {
                                var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, GetMemberExpression(parameter, item.Name)));
                                var contains = Expression.Call(GetMemberExpression(parameter, item.Name), "Contains", null,
                                    Expression.Convert(valueLamba.Body, property.PropertyType), Expression.Constant(StringComparison.CurrentCultureIgnoreCase, typeof(StringComparison)));
                                expression = Append(expression, Expression.AndAlso(nullCheck, contains));
                                break;
                            }
                        case Query.Operators.NotContains:
                            {
                                var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, GetMemberExpression(parameter, item.Name)));
                                var contains = Expression.Call(GetMemberExpression(parameter, item.Name), "Contains", null,
                                    Expression.Convert(valueLamba.Body, property.PropertyType), Expression.Constant(StringComparison.CurrentCultureIgnoreCase, typeof(StringComparison)));
                                var doesNotContain = Expression.Not(contains);
                                expression = Append(expression, Expression.AndAlso(nullCheck, doesNotContain));
                                break;
                            }
                        case Query.Operators.StartWith:
                            {
                                var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, GetMemberExpression(parameter, item.Name)));
                                var startsWith = Expression.Call(GetMemberExpression(parameter, item.Name), "StartsWith", null,
                                    Expression.Convert(valueLamba.Body, property.PropertyType), Expression.Constant(StringComparison.CurrentCultureIgnoreCase, typeof(StringComparison)));
                                expression = Append(expression, Expression.AndAlso(nullCheck, startsWith));
                                break;
                            }
                        case Query.Operators.EndWidth:
                            {
                                var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, GetMemberExpression(parameter, item.Name)));
                                var endsWith = Expression.Call(GetMemberExpression(parameter, item.Name), "EndsWith", null,
                                    Expression.Convert(valueLamba.Body, property.PropertyType), Expression.Constant(StringComparison.CurrentCultureIgnoreCase, typeof(StringComparison)));
                                expression = Append(expression, Expression.AndAlso(nullCheck, endsWith));
                                break;
                            }
                        case Query.Operators.Range:
                            {
                                Expression minExp = null, maxExp = null;
                                if (item.ValueMin != null)
                                {
                                    var minValue = Convert.ChangeType(item.ValueMin, realType);
                                    Expression<Func<object>> minValueLamda = () => minValue;
                                    minExp = Expression.GreaterThanOrEqual(GetMemberExpression(parameter, item.Name), Expression.Convert(minValueLamda.Body, property.PropertyType));
                                }
                                if (item.ValueMax != null)
                                {
                                    var maxValue = Convert.ChangeType(item.ValueMax, realType);
                                    Expression<Func<object>> maxValueLamda = () => maxValue;
                                    maxExp = Expression.LessThanOrEqual(GetMemberExpression(parameter, item.Name), Expression.Convert(maxValueLamda.Body, property.PropertyType));
                                }

                                if (minExp != null && maxExp != null)
                                {
                                    expression = Append(expression, Expression.AndAlso(minExp, maxExp));
                                }
                                else if (minExp != null)
                                {
                                    expression = Append(expression, minExp);
                                }
                                else if (maxExp != null)
                                {
                                    expression = Append(expression, maxExp);
                                }

                                break;
                            }
                    }
                }
                catch
                {

                }
            }
            if (expression == null)
            {
                return null;
            }
            return ((Expression<Func<T, bool>>)Expression.Lambda(expression, parameter));
        }
        /// <summary>
        /// 取最後一層的型別
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        private PropertyInfo GetLastProperty(string name, TypeInfo typeInfo)
        {
            var rtypeInfo = typeInfo;
            PropertyInfo PropertyInfo = typeInfo.GetProperty(name);
            var namearry = name.Split('.');
            for (int i = 0; i < namearry.Length; i++)
            {
                if (i == namearry.Length - 1)
                {
                    PropertyInfo = rtypeInfo.GetProperty(namearry[i]);
                }
                else
                {
                    var ntypeInfo = rtypeInfo.GetProperty(namearry[i]);
                    if (ntypeInfo != null)
                    {
                        rtypeInfo = ntypeInfo.PropertyType.GetTypeInfo();
                    }

                }

            }
            return PropertyInfo;
        }

        //makes expression for specific prop
    }
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName)
        {
            return QueryableHelper<T>.OrderBy(queryable, propertyName, false);
        }
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> queryable, string propertyName, bool desc)
        {
            return QueryableHelper<T>.OrderBy(queryable, propertyName, desc);
        }
        static class QueryableHelper<T>
        {
            private static MemberExpression GetMemberExpression(ParameterExpression parameter, string propName)
            {
                if (string.IsNullOrEmpty(propName)) return null;
                var propertiesName = propName.Split('.');
                if (propertiesName.Count() == 2)
                    return Expression.Property(Expression.Property(parameter, propertiesName[0]), propertiesName[1]);
                return Expression.Property(parameter, propName);
            }
            private static Dictionary<string, LambdaExpression> cache = new Dictionary<string, LambdaExpression>();
            public static IQueryable<T> OrderBy(IQueryable<T> queryable, string propertyName, bool desc)
            {
                dynamic keySelector = GetLambdaExpression(propertyName);
                return desc ? Queryable.OrderByDescending(queryable, keySelector) : Queryable.OrderBy(queryable, keySelector);
            }
            private static LambdaExpression GetLambdaExpression(string propertyName)
            {
                if (cache.ContainsKey(propertyName)) return cache[propertyName];
                var param = Expression.Parameter(typeof(T));
                var body = GetMemberExpression(param, propertyName);
                var keySelector = Expression.Lambda(body, param);
                cache[propertyName] = keySelector;
                return keySelector;
            }
        }
    }
    public static class IQueryableExtensions
    {
        private static object Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
        private static T Private<T>(this object obj, string privateField) => (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
        public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
            var relationalCommandCache = enumerator.Private("_relationalCommandCache");
            var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
            var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

            var sqlGenerator = factory.Create();
            var command = sqlGenerator.GetCommand(selectExpression);
            string sql = command.CommandText;
            return sql;
        }
    }
}
