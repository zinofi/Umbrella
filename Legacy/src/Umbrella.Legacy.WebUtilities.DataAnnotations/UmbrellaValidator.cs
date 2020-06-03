using System.Collections.Generic;
using System.Web.Mvc;
using Umbrella.DataAnnotations.BaseClasses;

namespace Umbrella.Legacy.WebUtilities.DataAnnotations
{
	/// <summary>
	/// Provides a model validator for validation attributes that extend <see cref="ModelAwareValidationAttribute"/>.
	/// </summary>
	/// <seealso cref="T:System.Web.Mvc.DataAnnotationsModelValidator{Umbrella.DataAnnotations.BaseClasses.ModelAwareValidationAttribute}" />
	public class UmbrellaValidator : DataAnnotationsModelValidator<ModelAwareValidationAttribute>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaValidator"/> class.
		/// </summary>
		/// <param name="metadata">The metadata for the model.</param>
		/// <param name="context">The controller context for the model.</param>
		/// <param name="attribute">The validation attribute for the model.</param>
		public UmbrellaValidator(ModelMetadata metadata, ControllerContext context, ModelAwareValidationAttribute attribute)
			: base(metadata, context, attribute)
		{
		}

		/// <inheritdoc />
		public override IEnumerable<ModelValidationResult> Validate(object container)
		{
			if (!Attribute.IsValid(Metadata.Model, container))
				yield return new ModelValidationResult { Message = ErrorMessage };
		}

		/// <inheritdoc />
		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
		{
			var result = new ModelClientValidationRule()
			{
				ValidationType = Attribute.ClientTypeName.ToLower(),
				ErrorMessage = ErrorMessage
			};

			foreach (var validationParam in Attribute.ClientValidationParameters)
				result.ValidationParameters.Add(validationParam);

			yield return result;
		}
	}
}