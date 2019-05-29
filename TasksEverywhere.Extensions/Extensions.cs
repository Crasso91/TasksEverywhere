using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TasksEverywhere.Extensions
{
    public static class GenericExtensions
    {
        public static T ToType<T>(this object _in)
        {
            return (T)Convert.ChangeType(_in, typeof(T));
        }

        public static List<PropertyInfo> GetProperties(this object _in)
        {
            return _in.GetType().GetProperties().ToList();
        }
        
        public static object Invoke(this object _in, string _name, object[] _params)
        {
            var method = _in.GetType().GetMethods().FirstOrDefault(x => x.Name.ToLower() == _name.ToLower());
            if(method != null) return method.Invoke(_in, _params);
            throw new NotImplementedException("Method " + _name + " not exists in object " + _in.GetType().Name);
        }

        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T ToEnum<T>(this object value)
        {
            return (T)Enum.Parse(typeof(T), value.ToString(), true);
        }

        public static string XmlSerialize(this object _in)
        {
            var xmlserializer = new XmlSerializer(_in.GetType());
            var stringWriter = new StringWriter();

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true
            };

            using (var writer = XmlWriter.Create(stringWriter, settings))
            {
                xmlserializer.Serialize(writer, _in, ns);
                return stringWriter.ToString();
            }
        }

        public static T XmlDeserialize<T>(this string _in)
        {
            var xmlserializer = new XmlSerializer(typeof(T));

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

            using (var reader = new StringReader(_in))
            {
                return (T)xmlserializer.Deserialize(reader);
            }
        }

        public static DateTime SetMidnight(this DateTime datetime)
        {
            return new DateTime(datetime.Year, datetime.Month, datetime.Day, 0, 0, 0);
        }

    }
}
