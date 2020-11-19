using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Umbrella.Utilities.Extensions
{
	public static class ValidationResultExtensions
	{
		public static string ToValidationSummaryMessage(this IEnumerable<ValidationResult> results, string message)
		{
			var messageBuilder = new StringBuilder(message).AppendLine().AppendLine();
			results.ForEach(x => messageBuilder.AppendLine(x.ErrorMessage));

			return messageBuilder.ToString();
		}
	}
}