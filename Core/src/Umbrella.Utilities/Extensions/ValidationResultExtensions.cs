using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Umbrella.Utilities.Extensions;

/// <summary>
/// Extension methods for use with the <see cref="ValidationResult"/> and collections if it.
/// </summary>
public static class ValidationResultExtensions
{
	/// <summary>
	/// Converts the specified <paramref name="results"/> and <paramref name="introMessage"/> to a single string.
	/// The <paramref name="introMessage"/> is displayed, followed by an empty line, followed by each message
	/// in the <paramref name="results"/>, one per line.
	/// </summary>
	/// <param name="results">The results.</param>
	/// <param name="introMessage">The introduction message displayed at the top of the validation results message.</param>
	/// <returns>The validation message.</returns>
	public static string ToValidationSummaryMessage(this IEnumerable<ValidationResult> results, string introMessage)
	{
		var messageBuilder = new StringBuilder(introMessage).AppendLine().AppendLine();
		results.ForEach(x => messageBuilder.AppendLine(x.ErrorMessage));

		return messageBuilder.ToString();
	}
}