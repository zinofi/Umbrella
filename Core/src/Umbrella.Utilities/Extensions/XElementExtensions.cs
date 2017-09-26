using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Umbrella.Utilities.Extensions
{
    public static class XElementExtensions
    {
        public static string GetAttributeValue(this XElement element, string name, bool required = true, string fallback = "")
            => GetAttributeValue<string>(element, name, required, fallback);

        public static T GetAttributeValue<T>(this XElement element, string name, bool required = true, T fallback = default)
        {
            Guard.ArgumentNotNull(element, nameof(element));
            Guard.ArgumentNotNullOrWhiteSpace(name, nameof(name));

            XAttribute attribute = element.Attribute(name);

            if (attribute == null && required)
                throw new Exception($"The {name} attribute of a {element.Name} element could not be found.");

            string attributeValue = attribute?.Value;

            var type = typeof(T);
            var typeInfo = type.GetTypeInfo();

            if (!string.IsNullOrEmpty(attributeValue))
            {
                if (typeInfo.IsPrimitive || type == typeof(string))
                {
                    if (typeInfo.IsPrimitive)
                        attributeValue = attributeValue.Replace(" ", "");

                    return (T)Convert.ChangeType(attributeValue, type);
                }
            }

            return type == typeof(string) && fallback == null
                ? (T)Convert.ChangeType(string.Empty, type)
                : fallback;
        }

        public static T GetAttributeEnumValue<T>(this XElement element, string name, bool required = true, T fallback = default)
            where T : struct
        {
            Guard.ArgumentNotNull(element, nameof(element));
            Guard.ArgumentNotNullOrWhiteSpace(name, nameof(name));

            XAttribute attribute = element.Attribute(name);

            if (attribute == null && required)
                throw new Exception($"The {name} attribute of a {element.Name} element could not be found.");

            string attributeValue = attribute?.Value?.Replace(" ", "");

            return !string.IsNullOrEmpty(attributeValue) && Enum.TryParse(attributeValue, true, out T output)
                ? output
                : fallback;
        }
    }
}