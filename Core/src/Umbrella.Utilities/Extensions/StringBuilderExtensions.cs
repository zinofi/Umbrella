using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.Utilities.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="StringBuilder"/> class.
	/// </summary>
	public static class StringBuilderExtensions
	{
		#region Private Members
		private static readonly ConcurrentDictionary<int, string> s_TabDictionary = new ConcurrentDictionary<int, string>();
		#endregion

		#region Public Methods		
		/// <summary>
		/// Finds the first index of the specified character from the specified start index.
		/// </summary>
		/// <param name="sb">The string builder.</param>
		/// <param name="value">The value to find.</param>
		/// <param name="startIndex">The start index.</param>
		/// <returns>The index of the value or -1 if it cannot be found.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="sb"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="startIndex"/> is less than 0.</exception>
		public static int IndexOf(this StringBuilder sb, char value, int startIndex = 0)
		{
			Guard.ArgumentNotNull(sb, nameof(sb));
			Guard.ArgumentInRange(startIndex, nameof(startIndex), 0);

			for (int i = startIndex; i < sb.Length; i++)
			{
				if (sb[i] == value)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Finds the index of the start of the specified string value using the specified start index.
		/// </summary>
		/// <param name="sb">The string builder.</param>
		/// <param name="value">The value to find.</param>
		/// <param name="startIndex">The start index.</param>
		/// <param name="ignoreCase">Performs a case insensitive search if set to <see langword="true"/>.</param>
		/// <returns>The index of the value or -1 if it cannot be found.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="sb"/> is null.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="startIndex"/> is less than 0.</exception>
		public static int IndexOf(this StringBuilder sb, string value, int startIndex = 0, bool ignoreCase = false)
		{
			Guard.ArgumentNotNull(sb, nameof(sb));
			Guard.ArgumentNotNull(value, nameof(value));
			Guard.ArgumentInRange(startIndex, nameof(startIndex), 0);

			int num3;
			int length = value.Length;
			int num2 = (sb.Length - length) + 1;

			if (ignoreCase == false)
			{
				for (int i = startIndex; i < num2; i++)
				{
					if (sb[i] == value[0])
					{
						num3 = 1;
						while ((num3 < length) && (sb[i + num3] == value[num3]))
						{
							num3++;
						}
						if (num3 == length)
						{
							return i;
						}
					}
				}
			}
			else
			{
				for (int j = startIndex; j < num2; j++)
				{
					if (char.ToLower(sb[j]) == char.ToLower(value[0]))
					{
						num3 = 1;
						while ((num3 < length) && (char.ToLower(sb[j + num3]) == char.ToLower(value[num3])))
						{
							num3++;
						}
						if (num3 == length)
						{
							return j;
						}
					}
				}
			}
			return -1;
		}

		public static bool Contains(this StringBuilder sb, string value) => sb.IndexOf(value) > -1;

		public static StringBuilder Trim(this StringBuilder sb) => Trim(sb, ' ');

		public static StringBuilder Trim(this StringBuilder sb, params char[] chars)
		{
			Guard.ArgumentNotNull(sb, nameof(sb));
			Guard.ArgumentNotNull(chars, nameof(chars));

			foreach (char c in chars)
			{
				sb.Trim(c);
			}

			return sb;
		}

		public static StringBuilder Trim(this StringBuilder sb, char c)
		{
			Guard.ArgumentNotNull(sb, nameof(sb));

			if (sb.Length != 0)
			{
				int length = 0;
				int num2 = sb.Length;
				while ((length < num2) && (sb[length] == c))
				{
					length++;
				}
				if (length > 0)
				{
					sb.Remove(0, length);
					num2 = sb.Length;
				}
				length = num2 - 1;
				while ((length > -1) && (sb[length] == c))
				{
					length--;
				}
				if (length < (num2 - 1))
				{
					sb.Remove(length + 1, (num2 - length) - 1);
				}
			}
			return sb;
		}

		public static bool StartsWith(this StringBuilder sb, char test) => sb.IndexOf(test) == 0;
		public static bool StartsWith(this StringBuilder sb, string test) => sb.IndexOf(test) == 0;

		public static bool StartsWith(this StringBuilder sb, string test, StringComparison comparison)
		{
			if (sb.Length < test.Length)
				return false;

			string end = sb.ToString(0, test.Length);

			return end.Equals(test, comparison);
		}

		public static bool EndsWith(this StringBuilder sb, char test) => sb.IndexOf(test) == sb.Length - 1;
		public static bool EndsWith(this StringBuilder sb, string test) => sb.IndexOf(test) == sb.Length - 1;

		public static bool EndsWith(this StringBuilder sb, string test, StringComparison comparison)
		{
			if (sb.Length < test.Length)
				return false;

			string end = sb.ToString(sb.Length - test.Length, test.Length);

			return end.Equals(test, comparison);
		}

		public static StringBuilder ConvertHtmlBrTagsToReplacement(this StringBuilder value, string replacement)
		{
			value.Replace("</br>", "")
				.Replace("<br>", replacement)
				.Replace("<br/>", replacement)
				.Replace("<br />", replacement);

			return value;
		}

		public static StringBuilder AppendLineWithTabIndent(this StringBuilder builder, string value, int tabCount)
		{
			string tabs = s_TabDictionary.GetOrAdd(tabCount, x => string.Join("", CreateTabArray(x)));

			return builder.AppendLine($"{tabs}{value}");
		}
		#endregion

		#region Private Methods
		private static IEnumerable<char> CreateTabArray(int length)
		{
			for (int i = 0; i < length; i++)
			{
				yield return '\t';
			}
		}
		#endregion
	}
}