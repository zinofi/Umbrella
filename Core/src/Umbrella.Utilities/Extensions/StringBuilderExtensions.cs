using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
	public static class StringBuilderExtensions
	{
        #region Private Members
        private static ConcurrentDictionary<int, string> s_TabDictionary = new ConcurrentDictionary<int, string>();
        #endregion

        #region Public Methods
        /// <summary>
        /// Get index of a char
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int IndexOf(this StringBuilder sb, char value) => IndexOf(sb, value, 0);

        /// <summary>
        /// Get index of a char starting from a given index
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="c"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int IndexOf(this StringBuilder sb, char value, int startIndex)
        {
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
        /// Get index of a string
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int IndexOf(this StringBuilder sb, string value) => IndexOf(sb, value, 0, false);

        /// <summary>
        /// Get index of a string from a given index
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int IndexOf(this StringBuilder sb, string value, int startIndex) => IndexOf(sb, value, startIndex, false);

        /// <summary>
        /// Get index of a string with case option
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="text"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static int IndexOf(this StringBuilder sb, string value, bool ignoreCase) => IndexOf(sb, value, 0, ignoreCase);

        /// <summary>
        /// Get index of a string from a given index with case option
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static int IndexOf(this StringBuilder sb, string value, int startIndex, bool ignoreCase)
        {
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

        public static StringBuilder Trim(this StringBuilder sb, char c)
        {
            if (sb.Length != 0)
            {
                int length = 0;
                int num2 = sb.Length;
                while ((sb[length] == c) && (length < num2))
                {
                    length++;
                }
                if (length > 0)
                {
                    sb.Remove(0, length);
                    num2 = sb.Length;
                }
                length = num2 - 1;
                while ((sb[length] == c) && (length > -1))
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

        public static bool EndsWith(this StringBuilder sb, string test) => EndsWith(sb, test, StringComparison.CurrentCulture);

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
