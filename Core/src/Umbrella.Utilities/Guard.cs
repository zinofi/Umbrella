using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.Utilities
{
    /// <summary>
    /// A static helper class that includes various parameter checking routines.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the given argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException">The value is null.</exception>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void ArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
                throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        /// Throws an exception if the tested string argument is null or an empty string.
        /// </summary>
        /// <exception cref="ArgumentNullException">The string value is null.</exception>
        /// <exception cref="ArgumentException">The string is empty.</exception>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void ArgumentNotNullOrWhiteSpace(string argumentValue, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argumentValue))
                throw new ArgumentException($"{argumentName} cannot be null, empty, or only whitespace.");
        }

        public static void ArgumentNotNullOrEmpty<T>(IList<T> argumentValue, string argumentName)
        {
            if (argumentValue?.Count == 0)
                throw new ArgumentException($"{argumentName} cannot be null or empty");
        }

        public static void ArgumentNotNullOrEmpty<T>(ICollection<T> argumentValue, string argumentName)
        {
            if (argumentValue?.Count == 0)
                throw new ArgumentException($"{argumentName} cannot be null or empty");
        }

        public static void ArgumentNotNullOrEmpty<T>(IEnumerable<T> argumentValue, string argumentName)
        {
            if (argumentValue?.Count() == 0)
                throw new ArgumentException($"{argumentName} cannot be null or empty");
        }

        /// <summary>
        /// Checks if the <paramref name="argumentValue"/> is of type <typeparamref name="T"/> or a type in it's type hierarchy.
        /// Calls <see cref="ArgumentNotNull(object, string)"/> internally.
        /// </summary>
        /// <typeparam name="T">The type to match.</typeparam>
        /// <param name="argumentValue">The object to test.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <exception cref="ArgumentNullException">The value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The value is not of type <typeparamref name="T"/> or a type in it's type hierarchy.</exception>
        public static void ArgumentOfType<T>(object argumentValue, string argumentName)
        {
            ArgumentNotNull(argumentValue, argumentName);

            if (argumentValue is T == false)
                throw new ArgumentOutOfRangeException($"{argumentName} is not of type {nameof(T)}: {typeof(T).FullName} or one of it's super types.");
        }

        /// <summary>
        /// Checks if the <paramref name="argumentValue"/> is axactly of type <typeparamref name="T"/>
        /// Calls <see cref="ArgumentNotNull(object, string)"/> internally.
        /// </summary>
        /// <typeparam name="T">The type to match.</typeparam>
        /// <param name="argumentValue">The object to test.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <exception cref="ArgumentNullException">The value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The value is not exactly of type <typeparamref name="T"/></exception>
        public static void ArgumentOfTypeExact<T>(object argumentValue, string argumentName)
        {
            ArgumentNotNull(argumentValue, argumentName);

            if (argumentValue.GetType() != typeof(T))
                throw new ArgumentOutOfRangeException($"{argumentName} is not exactly of type {nameof(T)}: {typeof(T).FullName}");
        }
    }
}