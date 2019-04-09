using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Boilerplate.Models.Enums;
using Boilerplate.Models.Exceptions;

namespace Boilerplate.Services
{
    public static class QueryHelper
    {
        private static readonly MethodInfo OrderByMethod =
            typeof(Queryable).GetMethods().Single(method =>
                method.Name == "OrderBy" && method.GetParameters().Length == 2);

        private static readonly MethodInfo OrderByDescendingMethod =
            typeof(Queryable).GetMethods().Single(method =>
                method.Name == "OrderByDescending" && method.GetParameters().Length == 2);

        public static bool PropertyExists<T>(string propertyName)
        {
            return typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase |
                                                       BindingFlags.Public | BindingFlags.Instance) != null;
        }

        public static IQueryable<T> OrderByProperty<T>(
            this IQueryable<T> source, string propertyName, OrderType orderType)
            => orderType == OrderType.Asc
                ? source.OrderByProperty(propertyName)
                : source.OrderByPropertyDescending(propertyName);

        public static IQueryable<T> OrderByProperty<T>(
            this IQueryable<T> source, string propertyName)
        {
            if (!PropertyExists<T>(propertyName))
                throw new BadRequestException($"{typeof(T).Name} doesn't contain property {propertyName}");

            var parameterExpression = Expression.Parameter(typeof(T));
            Expression orderByProperty = Expression.Property(parameterExpression, propertyName);
            var lambda = Expression.Lambda(orderByProperty, parameterExpression);
            var genericMethod =
                OrderByMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
            var ret = genericMethod.Invoke(null, new object[] { source, lambda });
            return (IQueryable<T>)ret;
        }

        public static IQueryable<T> OrderByPropertyDescending<T>(
            this IQueryable<T> source, string propertyName)
        {
            if (!PropertyExists<T>(propertyName))
                throw new BadRequestException($"{typeof(T).Name} doesn't contain property {propertyName}");

            var parameterExpression = Expression.Parameter(typeof(T));
            var orderByProperty = Expression.Property(parameterExpression, propertyName);
            var lambda = Expression.Lambda(orderByProperty, parameterExpression);
            var genericMethod =
                OrderByDescendingMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
            var ret = genericMethod.Invoke(null, new object[] { source, lambda });
            return (IQueryable<T>)ret;
        }
    }
}
