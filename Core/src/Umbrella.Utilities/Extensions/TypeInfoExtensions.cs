#if NET46 || NETSTANDARD1_5
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
    public static class TypeInfoExtensions
    {
        #region Public Static Methods
        public static PropertyInfo[] GetPublicOrPrivateProperties(this TypeInfo type) => type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        public static IEnumerable<Type> AssignableTo(this IEnumerable<Type> types, TypeInfo superType) => Enumerable.Where(types, new Func<Type, bool>(superType.IsAssignableFrom));

        public static IEnumerable<TypeInfo> Concrete(this IEnumerable<TypeInfo> types) => Enumerable.Where(types, type => !type.IsAbstract);

        public static bool IsNullableType(this TypeInfo type)
        {
            if (type.IsGenericType)
                return type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
            else
                return false;
        }

        public static bool CanBeNull(this TypeInfo type)
        {
            if (type.IsValueType)
                return IsNullableType(type);
            else
                return true;
        }

        public static Dictionary<int, string> GetEnumDictionary(this TypeInfo enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException($"{nameof(enumType)} is not an Enum.");

            return Enum.GetValues(enumType.AsType())
                .OfType<object>()
                .ToDictionary(x => Convert.ToInt32(x), x => Enum.GetName(enumType.AsType(), x));
        }

        public static bool IsAssignableToGenericType(this TypeInfo givenType, TypeInfo genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes.Select(x => x.GetTypeInfo()))
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition().GetTypeInfo() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition().GetTypeInfo() == genericType)
                return true;

            var baseTypeInfo = givenType.BaseType?.GetTypeInfo();

            if (baseTypeInfo == null)
                return false;

            return IsAssignableToGenericType(baseTypeInfo, genericType);
        }
        #endregion
    }
}
#endif