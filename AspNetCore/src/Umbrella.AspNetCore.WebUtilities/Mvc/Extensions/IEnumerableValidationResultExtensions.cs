using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.Extensions
{
	/// <summary>
	/// Extension methods that operate on <see cref="IEnumerable{ValidationResult}" /> whose elements are <see cref="ValidationResult"/> instances.
	/// </summary>
	public static class IEnumerableValidationResultExtensions
	{
		/// <summary>
		/// Converts the specified validation results to a <see cref="ModelStateDictionary"/> suitable for use with responses.
		/// </summary>
		/// <param name="validationResults">The validation results.</param>
		/// <returns>The <see cref="ModelStateDictionary" />.</returns>
		public static ModelStateDictionary ToModelStateDictionary(this IEnumerable<ValidationResult> validationResults)
		{
			var dictionary = new ModelStateDictionary();

			foreach (var item in validationResults)
			{
				var memberNames = item.MemberNames?.ToList();

				if (memberNames?.Count > 0)
				{
					foreach (string key in memberNames)
					{
						dictionary.AddModelError(key, item.ErrorMessage ?? string.Empty);
					}
				}
				else
				{
					if(!string.IsNullOrEmpty(item.ErrorMessage))
						dictionary.AddModelError(string.Empty, item.ErrorMessage);
				}
			}

			return dictionary;
		}
	}
}