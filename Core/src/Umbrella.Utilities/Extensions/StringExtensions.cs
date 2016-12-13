using Umbrella.Utilities.StringMetric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
	public static class StringExtensions
	{
		private const string c_HtmlTagPattern = @"<.*?>";
		private const string c_EllipsisPattern = @"[\.]+$";

		public static string Truncate(this string text, int maxLength)
		{
			//Ensure we strip out HTML tags
			if (!string.IsNullOrEmpty(text))
				text = text.StripHtml();

			if (text.Length < maxLength)
				return text;

			return AppendEllipsis(text.Substring(0, maxLength - 3));
		}

		public static string TruncateAtWord(this string value, int length)
		{
			//Ensure we strip out HTML tags
			if (!string.IsNullOrEmpty(value))
				value = value.StripHtml();

			if (value == null || value.Length < length || value.IndexOf(" ", length) == -1)
				return value;

			return AppendEllipsis(value.Substring(0, value.IndexOf(" ", length)));
		}

		public static string TrimNull(this string value) => !string.IsNullOrEmpty(value) ? value.Trim() : null;

		public static string StripNbsp(this string value) => !string.IsNullOrEmpty(value) ? value.Replace("&nbsp;", "") : null;

		public static bool IsValidLength(this string value, int minLength, int maxLength, bool allowNull = true)
		{
			if(string.IsNullOrWhiteSpace(value))
				return allowNull;
			
			return value.Length >= minLength && value.Length <= maxLength;
		}

		public static int GetEditDistance(this string first, string second, StringMetricAlgorithmType algorithm)
		{
			switch (algorithm)
			{
				default:
				case StringMetricAlgorithmType.DamerauLevenshtein:
					return StringMetricAlgorithms.GetEditDistanceDamerauLevenshtein(first, second);
			}
		}

        public static string StripHtml(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            Regex regex = new Regex(c_HtmlTagPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            return regex.Replace(value, string.Empty);
        }

        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            if (value.Length == 1)
                return value.ToLower();

            //If 1st char is already in lowercase, return the value untouched
            if (char.IsLower(value[0]))
                return value;

            char[] buffer = new char[value.Length];

            bool stop = false;

            for (int i = 0; i < value.Length; i++)
            {
                if (!stop)
                {
                    if (char.IsUpper(value[i]))
                    {
                        buffer[i] = char.ToLower(value[i]);
                        continue;
                    }
                    else if (i > 1)
                    {
                        //Encountered first lowercase char
                        //Check previous char and see if that was uppercase before we made it lowercase
                        char previous = value[i - 1];
                        if (char.IsUpper(previous))
                        {
                            buffer[i - 1] = previous;
                            stop = true;
                        }
                    }
                }

                buffer[i] = value[i];
            }

            return new string(buffer);
        }

        private static string AppendEllipsis(string value)
		{
            if (string.IsNullOrEmpty(value))
                return value;

            value += "...";

            Regex regex = new Regex(c_EllipsisPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            return regex.Replace(value, "...");
        }

        public static string ConvertHtmlBrTagsToLineBreaks(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            StringBuilder sb = new StringBuilder(value);
            sb.ConvertHtmlBrTagsToReplacement("\n");

            return sb.ToString();
        }

        public static string Clean(this string value, bool convertBrTagsToNl = false, bool trim = true, bool trimNewLines = true, bool stripHtml = true, bool stripNbsp = true, bool decodeHtmlEncodedLineBreaks = true)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            StringBuilder sb = new StringBuilder(value);

            if (convertBrTagsToNl)
                sb.ConvertHtmlBrTagsToReplacement("\n");
            else
                sb.ConvertHtmlBrTagsToReplacement("");

            if (trim)
                sb.Trim();

            bool trimAgain = false;

            if (decodeHtmlEncodedLineBreaks)
            {
                sb.Replace("&#10;", "\n")
                    .Replace("&#13;", "\r");
            }

            //Replace the following in strings
            sb.Replace("&amp;", "&");
            sb.Replace("&#39;", "'");
            sb.Replace("&quot;", "\"");
            sb.Replace("&#8216;", "'"); //Left quote
            sb.Replace("&#8217;", "'"); //Right quote

            if (trimNewLines)
            {
                sb.Trim('\r').Trim('\n');
                trimAgain = true;
            }

            if (stripNbsp)
            {
                sb.Replace("&nbsp", "");
                trimAgain = true;
            }

            string cleanedValue = sb.ToString();

            if (stripHtml)
                cleanedValue = cleanedValue.StripHtml();

            if (trim && trimAgain)
                cleanedValue = cleanedValue.Trim();

            return cleanedValue;
        }
    }
}