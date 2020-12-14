using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Umbrella.Utilities.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="Type"/> class.
	/// </summary>
	public static class TypeExtensions
    {
		#region Public Static Methods		
		/// <summary>
		/// Gets the public or private instance properties defined on the specified <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>The properties.</returns>
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

		/// <summary>
		/// Determines whether the type is permitted to have a null value.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns><see langword="true"/> if it is; otherwise <see langword="false"/>.</returns>
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

        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;

            if (baseType == null)
                return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

		public static (bool isEnumerable, Type? elementType) GetIEnumerableTypeData(this Type givenType)
		{
			if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				return (true, givenType.GetGenericArguments()[0]);

			var interfaceTypes = givenType.GetInterfaces();

			foreach (var it in interfaceTypes)
			{
				if (it.IsGenericType && it.GetGenericTypeDefinition() == typeof(IEnumerable<>))
					return (true, it.GetGenericArguments()[0]);
			}

			Type baseType = givenType.BaseType;

			if (baseType is null)
				return (false, null);

			return baseType.GetIEnumerableTypeData();
		}
		#endregion
	}
}