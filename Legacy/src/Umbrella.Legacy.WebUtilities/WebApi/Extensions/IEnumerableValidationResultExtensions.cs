using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http.ModelBinding;

namespace Umbrella.Legacy.WebUtilities.WebApi.Extensions;

public static class IEnumerableValidationResultExtensions
{
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
					dictionary.AddModelError(key, item.ErrorMessage);
				}
			}
			else
			{
				dictionary.AddModelError(string.Empty, item.ErrorMessage);
			}
		}

		return dictionary;
	}
}