using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Umbrella.Utilities
{
	// TODO TOOLKIT: Remove this when the CommunityToolkit NuGet packages come out.

	/// <summary>
	/// A static helper class that includes various parameter checking routines.
	/// </summary>
	public static class Guard
	{
		/// <summary>
		/// Throws <see cref="ArgumentException"/> if the given argument is not defined on the specified <see langword="enum"/> type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The type of the <see langword="enum"/>.</typeparam>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="argumentName">Name of the argument.</param>
		/// <exception cref="ArgumentException">The <paramref name="argumentValue"/> is not defined on the <see langword="enum"/> type <typeparamref name="T"/>.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentIsDefined<T>(T argumentValue, string argumentName)
			where T : struct, Enum
		{
			if (!Enum.IsDefined(typeof(T), argumentValue))
				throw new ArgumentException(argumentName);
		}

		/// <summary>
		/// Throws <see cref="ArgumentNullException"/> if the given argument is null.
		/// </summary>
		/// <exception cref="ArgumentNullException">The value is null.</exception>
		/// <param name="argumentValue">The argument value to test.</param>
		/// <param name="argumentName">The name of the argument to test.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentNotNull<T>(T argumentValue, string argumentName)
		{
			if (argumentValue is null)
				throw new ArgumentNullException(argumentName);
		}

		/// <summary>
		/// Throws an exception if the tested string argument is null, or an empty string.
		/// </summary>
		/// <exception cref="ArgumentNullException">The string value is null.</exception>
		/// <exception cref="ArgumentException">The string is empty.</exception>
		/// <param name="argumentValue">The argument value to test.</param>
		/// <param name="argumentName">The name of the argument to test.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentNotNullOrEmpty(string? argumentValue, string argumentName)
		{
			if (argumentValue is null)
				throw new ArgumentNullException($"{argumentName} cannot be null.");

			if (string.IsNullOrEmpty(argumentValue))
				throw new ArgumentException($"{argumentName} cannot be empty.");
		}

		/// <summary>
		/// Throws an exception if the tested string argument is null, whitespace, or an empty string.
		/// </summary>
		/// <exception cref="ArgumentNullException">The string value is null.</exception>
		/// <exception cref="ArgumentException">The string is empty or only whitespace.</exception>
		/// <param name="argumentValue">The argument value to test.</param>
		/// <param name="argumentName">The name of the argument to test.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentNotNullOrWhiteSpace(string? argumentValue, string argumentName)
		{
			if (argumentValue is null)
				throw new ArgumentNullException($"{argumentName} cannot be null.");

			if (string.IsNullOrWhiteSpace(argumentValue))
				throw new ArgumentException($"{argumentName} cannot be empty, or only whitespace.");
		}

		/// <summary>
		/// Throws an exception if the tested <see cref="IEnumerable{T}"/> argument is null or empty.
		/// </summary>
		/// <exception cref="ArgumentNullException">The <see cref="IEnumerable{T}"/> is null.</exception>
		/// <exception cref="ArgumentException">The <see cref="IEnumerable{T}"/> is empty.</exception>
		/// <param name="argumentValue">The argument value to test.</param>
		/// <param name="argumentName">The name of the argument to test.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentNotNullOrEmpty<T>(IEnumerable<T>? argumentValue, string argumentName)
		{
			if (argumentValue is null)
				throw new ArgumentNullException($"{argumentName} cannot be null.");

			if (argumentValue.Count() == 0)
				throw new ArgumentException($"{argumentName} cannot be empty.");
		}

		/// <summary>
		/// Throws an exception if the tested <see cref="Span{T}"/> argument empty.
		/// </summary>
		/// <typeparam name="T">The type of the items in the <see cref="Span{T}"/> instance.</typeparam>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="argumentName">Name of the argument.</param>
		/// <exception cref="ArgumentException">The <see cref="Span{T}"/> is empty.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentNotEmpty<T>(in Span<T> argumentValue, string argumentName)
		{
			if (argumentValue.Length == 0)
				throw new ArgumentException($"{argumentName} cannot be empty.");
		}

		/// <summary>
		/// Throws an exception if the tested <see cref="ReadOnlySpan{T}"/> argument empty.
		/// </summary>
		/// <typeparam name="T">The type of the items in the <see cref="ReadOnlySpan{T}"/> instance.</typeparam>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="argumentName">Name of the argument.</param>
		/// <exception cref="ArgumentException">The <see cref="ReadOnlySpan{T}"/> is empty.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentNotEmpty<T>(in ReadOnlySpan<T> argumentValue, string argumentName)
		{
			if (argumentValue.Length == 0)
				throw new ArgumentException($"{argumentName} cannot be empty.");
		}

		/// <summary>
		/// Checks if the <paramref name="argumentValue"/> is of type <typeparamref name="T"/> or a type in it's type hierarchy.
		/// Calls <see cref="ArgumentNotNull{T}(T, string)"/> internally.
		/// </summary>
		/// <typeparam name="T">The type to match.</typeparam>
		/// <param name="argumentValue">The object to test.</param>
		/// <param name="argumentName">The name of the argument.</param>
		/// <param name="customMessage">A custom message which is appended to the default error message.</param>
		/// <exception cref="ArgumentNullException">The value is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The value is not of type <typeparamref name="T"/> or a type in it's type hierarchy.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentOfType<T>(object? argumentValue, string argumentName, string customMessage = "")
		{
			ArgumentNotNull(argumentValue, argumentName);

			if (argumentValue is T == false)
				throw new ArgumentOutOfRangeException($"{argumentName} is not of type {nameof(T)}: {typeof(T).FullName} or one of it's super types. {customMessage}");
		}

		/// <summary>
		/// Checks if the <paramref name="argumentValue"/> is axactly of type <typeparamref name="T"/>
		/// Calls <see cref="ArgumentNotNull{T}(T, string)"/> internally.
		/// </summary>
		/// <typeparam name="T">The type to match.</typeparam>
		/// <param name="argumentValue">The object to test.</param>
		/// <param name="argumentName">The name of the argument.</param>
		/// <param name="customMessage">A custom message which is appended to the default error message.</param>
		/// <exception cref="ArgumentNullException">The value is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">The value is not exactly of type <typeparamref name="T"/></exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentOfTypeExact<T>(object? argumentValue, string argumentName, string customMessage = "")
		{
			ArgumentNotNull(argumentValue, argumentName);

			if (argumentValue!.GetType() != typeof(T))
				throw new ArgumentOutOfRangeException($"{argumentName} is not exactly of type {nameof(T)}: {typeof(T).FullName}. {customMessage}");
		}

		/// <summary>
		/// Checks if a string is between a minimum and maximum length.
		/// </summary>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="argumentName">Name of the argument.</param>
		/// <param name="minLength">The optional minimum length.</param>
		/// <param name="maxLength">The optional maximum length.</param>
		/// <exception cref="ArgumentNullException">The argument is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The string must have a minimum length of {minLength}
		/// or
		/// The string must have a maximum length of {maxLength}
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentLengthInRange(string? argumentValue, string argumentName, int? minLength = null, int? maxLength = null)
		{
			ArgumentNotNull(argumentValue, argumentName);

			if (minLength.HasValue && argumentValue!.Length < minLength)
				throw new ArgumentOutOfRangeException(argumentName, $"The string must have a minimum length of {minLength}");

			if (maxLength.HasValue && argumentValue!.Length > maxLength)
				throw new ArgumentOutOfRangeException(argumentName, $"The string must have a maximum length of {maxLength}");
		}

		/// <summary>
		/// Checks if a value is within a specified range.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="argumentName">Name of the argument.</param>
		/// <param name="min">The minimum.</param>
		/// <param name="max">The maximum.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The value must be greater than or equal to {min}
		/// or
		/// The value must be less than or equal to {max}
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentInRange<T>(T argumentValue, string argumentName, T? min = null, T? max = null)
			where T : struct, IComparable<T>
		{
			if (min.HasValue && argumentValue.CompareTo(min.Value) < 0)
				throw new ArgumentOutOfRangeException(argumentName, $"The value must be greater than or equal to {min}.");

			if (max.HasValue && argumentValue.CompareTo(max.Value) > 0)
				throw new ArgumentOutOfRangeException(argumentName, $"The value must be less than or equal to {max}.");
		}

		/// <summary>
		/// Checks if a value is within a specified range.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="argumentValue">The argument value.</param>
		/// <param name="argumentName">Name of the argument.</param>
		/// <param name="min">The minimum.</param>
		/// <param name="max">The maximum.</param>
		/// <param name="allowNull">if set to <c>true</c> [allow null].</param>
		/// <exception cref="ArgumentNullException">The argument value provided cannot be null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The value must be greater than or equal to {min}
		/// or
		/// The value must be less than or equal to {max}
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ArgumentInRange<T>(T? argumentValue, string argumentName, T? min = null, T? max = null, bool allowNull = false)
			where T : struct, IComparable<T>
		{
			if (!argumentValue.HasValue && !allowNull)
				throw new ArgumentNullException(argumentName, "The argument value provided cannot be null.");

			if (argumentValue.HasValue)
			{
				if (min.HasValue && argumentValue.Value.CompareTo(min.Value) < 0)
					throw new ArgumentOutOfRangeException(argumentName, $"The value must be greater than or equal to {min}.");

				if (max.HasValue && argumentValue.Value.CompareTo(max.Value) > 0)
					throw new ArgumentOutOfRangeException(argumentName, $"The value must be less than or equal to {max}.");
			}
		}

		/// <summary>
		/// Ensures the <paramref name="min"/> value is less than or equal to the <paramref name="max"/> value.
		/// </summary>
		/// <typeparam name="T">The type of the arguments.</typeparam>
		/// <param name="min">The minimum.</param>
		/// <param name="max">The maximum.</param>
		/// <param name="minArgumentName">The name of the <paramref name="min"/> argument.</param>
		/// <param name="maxArgumentName">The name of the <paramref name="max"/> argument.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown if the <paramref name="min"/> is greater than the <paramref name="max"/> value.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void EnsureMinLtEqMax<T>(T min, T max, string minArgumentName = "min", string maxArgumentName = "max")
			where T : struct, IComparable<T>
		{
			ArgumentNotNullOrWhiteSpace(minArgumentName, nameof(minArgumentName));
			ArgumentNotNullOrWhiteSpace(maxArgumentName, nameof(maxArgumentName));

			if (min.CompareTo(max) == 1)
				throw new ArgumentOutOfRangeException($"{minArgumentName} = {min} must be less than {maxArgumentName} = {max}");
		}
	}
}