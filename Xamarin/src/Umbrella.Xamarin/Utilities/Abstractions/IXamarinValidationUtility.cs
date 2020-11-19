using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Utilities.Abstractions
{
	/// <summary>
	/// A utililty class used to enable model validation of types annotated with <see cref="System.ComponentModel.DataAnnotations"/> attributes
	/// and then update the UI to reflect any validation errors.
	/// </summary>
	public interface IXamarinValidationUtility
	{
		/// <summary>
		/// Hides the validation fields.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="page">The page.</param>
		void HideValidationFields(object model, Page? page);

		/// <summary>
		/// Validates the model.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="page">The page.</param>
		/// <param name="resultsModifier">The results modifier.</param>
		/// <param name="deep">if set to <c>true</c> [deep].</param>
		/// <param name="attachInlineValiation">if set to <c>true</c> attaches event handlers to UI views to enable validation rules to be re-evaulated in response to changes, e.g. user input.</param>
		/// <returns>The validation result.</returns>
		(bool isValid, IReadOnlyCollection<ValidationResult> results) ValidateModel(object model, Page? page, Func<List<ValidationResult>, IReadOnlyCollection<ValidationResult>>? resultsModifier = null, bool deep = false, bool attachInlineValiation = true);

		/// <summary>
		/// Validates the property.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="property">The property.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="page">The page.</param>
		/// <param name="resultsModifier">The results modifier.</param>
		/// <param name="attachInlineValiation">if set to <c>true</c> attaches event handlers to UI views to enable validation rules to be re-evaulated in response to changes, e.g. user input.</param>
		/// <returns>The validation result.</returns>
		(bool isValid, IReadOnlyCollection<ValidationResult> results) ValidateProperty(object model, object? property, string propertyName, Page? page, Func<List<ValidationResult>, IReadOnlyCollection<ValidationResult>>? resultsModifier = null, bool attachInlineValiation = true);
	}
}