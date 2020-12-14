using System;
using System.Xml.Linq;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.Utilities.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="XElement"/> class.
	/// </summary>
	public static class XElementExtensions
    {
        public static string? GetAttributeValue(this XElement element, string name, bool required = true, string fallback = "")
            => GetAttributeValue<string>(element, name, required, fallback);

        public static T? GetAttributeValue<T>(this XElement element, string name, bool required = true, T fallback = default)
        {
            Guard.ArgumentNotNull(element, nameof(element));
            Guard.ArgumentNotNullOrWhiteSpace(name, nameof(name));

            XAttribute? attribute = element.Attribute(name);

            if (attribute is null && required)
				throw new UmbrellaException($"The {name} attribute of a {element.Name} element could not be found.");

            string? attributeValue = attribute?.Value;

            var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            if (!string.IsNullOrWhiteSpace(attributeValue))
            {
                if (type.IsPrimitive || type == typeof(decimal) || type == typeof(string))
                {
                    string cleanedAttributeValue = type.IsPrimitive || type == typeof(decimal)
                        ? attributeValue!.Replace(" ", "")
                        : attributeValue!;

                    try
                    {
                        return (T)Convert.ChangeType(cleanedAttributeValue, type);
                    }
                    catch (Exception exc)
                    {
                        throw new UmbrellaException($"The {name} attribute of a {element.Name} element with value {attributeValue} could not be converted to type {type.FullName}.", exc);
                    }
                }
            }

            return type == typeof(string) && fallback == null
                ? (T)Convert.ChangeType(string.Empty, type)
                : fallback;
        }

        public static T GetAttributeEnumValue<T>(this XElement element, string name, bool required = true, T fallback = default)
            where T : struct, Enum
        {
            Guard.ArgumentNotNull(element, nameof(element));
            Guard.ArgumentNotNullOrWhiteSpace(name, nameof(name));

            XAttribute? attribute = element.Attribute(name);

            if (attribute is null && required)
                throw new UmbrellaException($"The {name} attribute of a {element.Name} element could not be found.");

            string? attributeValue = attribute?.Value?.Replace(" ", "");

            return !string.IsNullOrEmpty(attributeValue) && Enum.TryParse(attributeValue, true, out T output)
                ? output
                : fallback;
        }

        public static (bool success, string? value) TryGetAttributeValue(this XElement element, string name, bool required = true, string fallback = "")
        {
            try
            {
                return (true, GetAttributeValue(element, name, required, fallback));
            }
            catch(Exception)
            {
                return (false, fallback);
            }
        }

        public static (bool success, T? value) TryGetAttributeValue<T>(this XElement element, string name, bool required = true, T fallback = default)
        {
            try
            {
                return (true, GetAttributeValue(element, name, required, fallback));
            }
            catch (Exception)
            {
                return (false, fallback);
            }
        }

        public static (bool success, T value) TryGetAttributeEnumValue<T>(this XElement element, string name, bool required = true, T fallback = default)
            where T : struct, Enum
        {
            try
            {
                return (true, GetAttributeEnumValue(element, name, required, fallback));
            }
            catch (Exception)
            {
                return (false, fallback);
            }
        }
    }
}