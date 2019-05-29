using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TasksEverywhereTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var obj = ReflectionUtil.GetInstanceOf<InternalObject>();
            var obj1 = ReflectionUtil.GetInstanceOf(typeof(InternalObject));

            ReflectionUtil.SetPropertyValue(obj, "prop1", "soncazzo");
            ReflectionUtil.SetPropertyValue(obj, "prop2", 12);
            ReflectionUtil.SetPropertyValue(obj1, "prop1", "OOOO!");
            ReflectionUtil.SetPropertyValue(obj1, "prop2", 45);

            var obj_prop1 = ReflectionUtil.GetPropertyValue<InternalObject, int>(obj, "prop1");
            var obj_prop = ReflectionUtil.GetPropertyValue<InternalObject, string>(obj, "prop");
            var obj1_prop1 = ReflectionUtil.GetPropertyValue(obj, "prop1");
            var obj1_prop = ReflectionUtil.GetPropertyValue(obj, "prop");
        }

        class InternalObject
        {
            public string prop1 { get; set; }
            public int prop2 { get; set; }
        }

        class ReflectionUtil
        {
            public static T GetInstanceOf<T>()
            {
                return (T)Assembly.GetExecutingAssembly().CreateInstance(typeof(T).Name);
            }

            public static IEnumerable<PropertyInfo> GetPropertiesOf<T>(T instance)
            {
                return instance.GetType().GetProperties();
            }

            public static PropertyInfo GetPropertyOf<T>(T instance, string propertyName)
            {
                return ReflectionUtil.GetPropertiesOf<T>(instance).FirstOrDefault(x => x.Name == propertyName);
            }

            public static T1 GetPropertyValue<T, T1>(T instance, string propertyName)
            {
                var property = ReflectionUtil.GetPropertyOf<T>(instance, propertyName);
                return (T1)property.GetValue(instance);
            }

            public static object GetInstanceOf(Type type)
            {
                return Assembly.GetExecutingAssembly().CreateInstance(type.Name);
            }

            public static IEnumerable<PropertyInfo> GetPropertiesOf(object instance)
            {
                return instance.GetType().GetProperties();
            }

            public static PropertyInfo GetPropertyOf(object instance, string propertyName)
            {
                return ReflectionUtil.GetPropertiesOf(instance).FirstOrDefault(x => x.Name == propertyName);
            }

            public static object GetPropertyValue(object instance, string propertyName)
            {
                var property = ReflectionUtil.GetPropertyOf(instance, propertyName);
                return property.GetValue(instance);
            }

            public static void SetPropertyValue(object instance, string propertyName, object value)
            {
                var property = ReflectionUtil.GetPropertyOf(instance, propertyName);
                property.SetValue(instance, value);
            }
        }
    }
}
