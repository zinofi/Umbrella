using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.Utilities
{
    // TODO: Look at possible making the methods that accept objects be generic instead to avoid accidently boxing something.
    // Or just add a generic overload and then allow the caller to figure it out and hope they don't do stupid stuff!
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
            if (argumentValue == null)
                throw new ArgumentNullException($"{argumentName} cannot be null.");

            if (string.IsNullOrWhiteSpace(argumentValue))
                throw new ArgumentException($"{argumentName} cannot be empty, or only whitespace.");
        }

        /// <summary>
        /// Throws an exception if the tested <see cref="IList{T}"/> argument is null or empty.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <see cref="IList{T}"/> is null.</exception>
        /// <exception cref="ArgumentException">The <see cref="IList{T}"/> is empty.</exception>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void ArgumentNotNullOrEmpty<T>(IList<T> argumentValue, string argumentName)
        {
            if (argumentValue == null)
                throw new ArgumentNullException($"{argumentName} cannot be null.");

            if (argumentValue.Count == 0)
                throw new ArgumentException($"{argumentName} cannot be empty.");
        }

        /// <summary>
        /// Throws an exception if the tested <see cref="ICollection{T}"/> argument is null or empty.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <see cref="ICollection{T}"/> is null.</exception>
        /// <exception cref="ArgumentException">The <see cref="ICollection{T}"/> is empty.</exception>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void ArgumentNotNullOrEmpty<T>(ICollection<T> argumentValue, string argumentName)
        {
            if (argumentValue == null)
                throw new ArgumentNullException($"{argumentName} cannot be null.");

            if (argumentValue.Count == 0)
                throw new ArgumentException($"{argumentName} cannot be empty.");
        }

        /// <summary>
        /// Throws an exception if the tested <see cref="IEnumerable{T}"/> argument is null or empty.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <see cref="IEnumerable{T}"/> is null.</exception>
        /// <exception cref="ArgumentException">The <see cref="IEnumerable{T}"/> is empty.</exception>
        /// <param name="argumentValue">The argument value to test.</param>
        /// <param name="argumentName">The name of the argument to test.</param>
        public static void ArgumentNotNullOrEmpty<T>(IEnumerable<T> argumentValue, string argumentName)
        {
            if (argumentValue == null)
                throw new ArgumentNullException($"{argumentName} cannot be null.");

            if (argumentValue.Count() == 0)
                throw new ArgumentException($"{argumentName} cannot be empty.");
        }

        /// <summary>
        /// Checks if the <paramref name="argumentValue"/> is of type <typeparamref name="T"/> or a type in it's type hierarchy.
        /// Calls <see cref="ArgumentNotNull(object, string)"/> internally.
        /// </summary>
        /// <typeparam name="T">The type to match.</typeparam>
        /// <param name="argumentValue">The object to test.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <param name="customMessage">A custom message which is appended to the default error message.</param>
        /// <exception cref="ArgumentNullException">The value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The value is not of type <typeparamref name="T"/> or a type in it's type hierarchy.</exception>
        public static void ArgumentOfType<T>(object argumentValue, string argumentName, string customMessage = "")
        {
            ArgumentNotNull(argumentValue, argumentName);

            if (argumentValue is T == false)
                throw new ArgumentOutOfRangeException($"{argumentName} is not of type {nameof(T)}: {typeof(T).FullName} or one of it's super types. {customMessage}");
        }

        /// <summary>
        /// Checks if the <paramref name="argumentValue"/> is axactly of type <typeparamref name="T"/>
        /// Calls <see cref="ArgumentNotNull(object, string)"/> internally.
        /// </summary>
        /// <typeparam name="T">The type to match.</typeparam>
        /// <param name="argumentValue">The object to test.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <param name="customMessage">A custom message which is appended to the default error message.</param>
        /// <exception cref="ArgumentNullException">The value is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The value is not exactly of type <typeparamref name="T"/></exception>
        public static void ArgumentOfTypeExact<T>(object argumentValue, string argumentName, string customMessage = "")
        {
            ArgumentNotNull(argumentValue, argumentName);

            if (argumentValue.GetType() != typeof(T))
                throw new ArgumentOutOfRangeException($"{argumentName} is not exactly of type {nameof(T)}: {typeof(T).FullName}. {customMessage}");
        }
    }
}