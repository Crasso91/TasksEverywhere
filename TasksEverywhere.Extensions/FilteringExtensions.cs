using TasksEverywhere.Extensions;
using TasksEverywhere.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Extensions.Extensions
{
    public enum CompareSign
    {
        Equals,
        NotEquals,
        Greater,
        Lower,
        GreaterEquals,
        LowerEquals
    }

    public static class FilteringExtensions
    {
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> entities, IDictionary<string, object> filtersValues)
            where T : class
        {

            var filters = new Dictionary<string, Func<T, object, bool>>();
            int? top = null;
            var property = string.Empty;
            var comparer = CompareSign.Equals;

            foreach (var filterValue in filtersValues)
            {
                if (filterValue.Key == "top")
                {
                    top = filterValue.Value.ToType<int>();
                    continue;
                }
                var propertyAndComparer = filterValue.Key.Split('#');
                property = propertyAndComparer[0];

                filters[property] = (obj, val) =>
                {
                    if (propertyAndComparer.Count() > 1) comparer = propertyAndComparer[1].ToEnum<CompareSign>();

                    var propInfo = obj.GetType().GetProperty(property);
                    var propValue = Convert.ChangeType(propInfo.GetValue(obj), propInfo.PropertyType);
                    return propValue.Compare(comparer, val); //Object.Equals(propValue, val);
                };
            }
            foreach(var value in filtersValues)
            {
                var propertyAndComparer = value.Key.Split('#');
                var key = propertyAndComparer[0];
                entities = entities.Where(x => filters[key](x, value.Value));
            }

            if (top.HasValue)
            {
                entities.Take(top.Value);
            }
            return entities;

        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<T> entities, DbFiltersCollection filters)
        {
            var filtersMethods = new Dictionary<string, Func<T, object, bool>>();

            foreach (DbFilter filter in filters.GetFilters())
            {
                filtersMethods[filter.PropertyName] = (obj, val) =>
                {
                    var propInfo = obj.GetType().GetProperty(filter.PropertyName);
                    var propValue = propInfo.GetValue(obj);
                    return propValue.Compare(filter.Sign, val);

                };
            }

            foreach(DbFilter filter in filters.GetFilters())
            {
                entities.Where(x => filtersMethods[filter.PropertyName](x, filter.Value));
            }

            if (filters.HasTop())
                entities.Take(filters.GetTopFilter().Number);

            return entities;
        }

        public static bool Compare(this object propValue, CompareSign compareSign, object value)
        {
            switch (compareSign)
            {
                case CompareSign.Equals:
                    return Object.Equals(propValue, value);
                case CompareSign.NotEquals:
                    return !Object.Equals(propValue, value);
                default:
                    return Object.Equals(propValue, value);
            }
        }

        public static bool Compare(this DateTime value1, CompareSign compareSign, DateTime value2)
        {
            switch (compareSign)
            {
                case CompareSign.Equals:
                    return Object.Equals(value1, value2);
                case CompareSign.NotEquals:
                    return !Object.Equals(value1, value2);
                case CompareSign.Greater:
                    return value1 > value2;
                case CompareSign.Lower:
                    return value1 > value2;
                case CompareSign.GreaterEquals:
                    return value1 >= value2;
                case CompareSign.LowerEquals:
                    return value1 <= value2;
                default:
                    return true;
            }
        }

        public static bool Compare(this int value1, CompareSign compareSign, int value2)
        {
            switch (compareSign)
            {
                case CompareSign.Equals:
                    return Object.Equals(value1, value2);
                case CompareSign.NotEquals:
                    return !Object.Equals(value1, value2);
                case CompareSign.Greater:
                    return value1 > value2;
                case CompareSign.Lower:
                    return value1 > value2;
                case CompareSign.GreaterEquals:
                    return value1 >= value2;
                case CompareSign.LowerEquals:
                    return value1 <= value2;
                default:
                    return true;
            }
        }


        public static bool Compare(this decimal value1, CompareSign compareSign, decimal value2)
        {
            switch (compareSign)
            {
                case CompareSign.Equals:
                    return Object.Equals(value1, value2);
                case CompareSign.NotEquals:
                    return !Object.Equals(value1, value2);
                case CompareSign.Greater:
                    return value1 > value2;
                case CompareSign.Lower:
                    return value1 > value2;
                case CompareSign.GreaterEquals:
                    return value1 >= value2;
                case CompareSign.LowerEquals:
                    return value1 <= value2;
                default:
                    return true;
            }
        }
    }
}
