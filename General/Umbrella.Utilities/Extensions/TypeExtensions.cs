using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
    public static class TypeExtensions
    {
        #region Public Static Methods
        public static PropertyInfo[] GetPublicOrPrivateProperties(this Type type) => type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        public static IEnumerable<Type> AssignableTo(this IEnumerable<Type> types, Type superType) => Enumerable.Where(types, new Func<Type, bool>(superType.IsAssignableFrom));

        public static IEnumerable<Type> Concrete(this IEnumerable<Type> types) => Enumerable.Where(types, type => !type.IsAbstract);

        public static bool IsNullableType(this Type type)
        {
            if (type.IsGenericType)
                return type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
            else
                return false;
        }

        public static bool CanBeNull(this Type type)
        {
            if (type.IsValueType)
                return IsNullableType(type);
            else
                return true;
        }

        public static Dictionary<int, string> GetEnumDictionary(this Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException($"{nameof(enumType)} is not an Enum.");

            return Enum.GetValues(enumType)
                .OfType<object>()
                .ToDictionary(x => Convert.ToInt32(x), x => Enum.GetName(enumType, x));
        }
        #endregion
    }
}