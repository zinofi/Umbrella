using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Umbrella.TypeScript.Generators;
using Umbrella.Utilities.Extensions;

namespace Umbrella.TypeScript.Aurelia
{
	public class AureliaClassGenerator : BaseClassGenerator
	{
		#region Private Constants
		/// <summary>
		/// This is the same regex used inside the implementation of the EmailAddressAttribute. Found this by checking the Microsoft Reference Source.
		/// This string has been double escaped so it is output in a JavaScript friendly format.
		/// </summary>
		private const string c_RegexEmail = "new RegExp(\"^((([a-z]|\\\\d|[!#\\\\$%&'\\\\*\\\\+\\\\-\\\\/=\\\\?\\\\^_`{\\\\|}~]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])+(\\\\.([a-z]|\\\\d|[!#\\\\$%&'\\\\*\\\\+\\\\-\\\\/=\\\\?\\\\^_`{\\\\|}~]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])+)*)|((\\\\x22)((((\\\\x20|\\\\x09)*(\\\\x0d\\\\x0a))?(\\\\x20|\\\\x09)+)?(([\\\\x01-\\\\x08\\\\x0b\\\\x0c\\\\x0e-\\\\x1f\\\\x7f]|\\\\x21|[\\\\x23-\\\\x5b]|[\\\\x5d-\\\\x7e]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])|(\\\\\\\\([\\\\x01-\\\\x09\\\\x0b\\\\x0c\\\\x0d-\\\\x7f]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF]))))*(((\\\\x20|\\\\x09)*(\\\\x0d\\\\x0a))?(\\\\x20|\\\\x09)+)?(\\\\x22)))@((([a-z]|\\\\d|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])|(([a-z]|\\\\d|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])([a-z]|\\\\d|-|\\\\.|_|~|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])*([a-z]|\\\\d|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])))\\\\.)+(([a-z]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])|(([a-z]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])([a-z]|\\\\d|-|\\\\.|_|~|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])*([a-z]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])))\\\\.?$\", \"i\")";
		#endregion

		#region Overridden Properties
		public override TypeScriptOutputModelType OutputModelType => TypeScriptOutputModelType.AureliaClass;
		protected override bool SupportsValidationRules => true;
		protected override TypeScriptOutputModelType InterfaceModelType => TypeScriptOutputModelType.AureliaInterface;
		#endregion

		#region Overridden Methods
		protected override void WriteStart(Type modelType, StringBuilder builder) => base.WriteStart(modelType, builder);

		protected override void WriteValidationRules(PropertyInfo propertyInfo, TypeScriptMemberInfo tsInfo, StringBuilder validationBuilder)
		{
			//Get all types that are either of type ValidationAttribute or derive from it
			//However, specifically exclude instances of type DataTypeAttribute
			var lstValidationAttribute = propertyInfo.GetCustomAttributes<ValidationAttribute>().Where(x => x.GetType() != typeof(DataTypeAttribute)).ToList();

			if (lstValidationAttribute.Count == 0)
				return;

			string declaringTypeName = propertyInfo.DeclaringType.Name;
			string ensureProperty = $".ensure(\"{tsInfo.Name.ToCamelCaseInvariant()}\")";

			for (int i = 0; i < lstValidationAttribute.Count; i++)
			{
				var validationAttribute = lstValidationAttribute[i];

				string message = $"\"{validationAttribute.FormatErrorMessage(propertyInfo.Name)}\"";

				if (validationAttribute is RequiredAttribute)
				{
					validationBuilder.AppendLineWithTabIndent(ensureProperty, 4)
						.AppendLineWithTabIndent($".required()", 4);
				}
				else if (validationAttribute is CompareAttribute compareAttribute)
				{
					string otherPropertyName = compareAttribute.OtherProperty.ToCamelCaseInvariant();

					validationBuilder.AppendLineWithTabIndent(ensureProperty, 4)
						.AppendLineWithTabIndent($".satisfies((value: string, obj: {declaringTypeName}) => value === obj.{otherPropertyName})", 4);
				}
				else if (validationAttribute is CreditCardAttribute)
				{
					//Do nothing - fallback to server validation
				}
				else if (validationAttribute is EmailAddressAttribute)
				{
					validationBuilder.AppendLineWithTabIndent(ensureProperty, 4)
						.AppendLineWithTabIndent($".matches({c_RegexEmail})", 4);
				}
				else if (validationAttribute is FileExtensionsAttribute)
				{
					//Do nothing - fallback to server validation
				}
				else if (validationAttribute is MaxLengthAttribute maxLengthAttribute)
				{
					validationBuilder.AppendLineWithTabIndent(ensureProperty, 4)
						.AppendLineWithTabIndent($".maxLength({maxLengthAttribute.Length})", 4);
				}
				else if (validationAttribute is MinLengthAttribute minLengthAttribute)
				{
					validationBuilder.AppendLineWithTabIndent(ensureProperty, 4)
						.AppendLineWithTabIndent($".minLength({minLengthAttribute.Length})", 4);
				}
				else if (validationAttribute is PhoneAttribute)
				{
					//Do nothing - fallback to server validation
				}
				else if (validationAttribute is RangeAttribute rangeAttribute)
				{
					// TODO - NEVER: Remove support for Aurelia if Blazor turns out ok. Can always add back.
					validationBuilder.AppendLineWithTabIndent(ensureProperty, 4)
						.AppendLineWithTabIndent($".satisfies((value: number, obj: {declaringTypeName}) => obj.{tsInfo.Name} >= {rangeAttribute.Minimum} && obj.{tsInfo.Name} <= {rangeAttribute.Maximum})", 4);
				}
				else if (validationAttribute is RegularExpressionAttribute regexAttribute)
				{
					validationBuilder.AppendLineWithTabIndent(ensureProperty, 4)
						.AppendLineWithTabIndent($".matches(new RegExp({regexAttribute.Pattern})", 4);
				}
				else if (validationAttribute is StringLengthAttribute stringLengthAttribute)
				{
					validationBuilder.AppendLineWithTabIndent(ensureProperty, 4)
						.AppendLineWithTabIndent($".minLength({stringLengthAttribute.MinimumLength})", 4)
						.AppendLineWithTabIndent($".maxLength({stringLengthAttribute.MaximumLength})", 4);
				}
				else if (validationAttribute is UrlAttribute)
				{
					//Do nothing - fallback to server validation
				}
				else
				{
					//If we get this far then we are dealing with an attribute outside the above items
					//Don't throw an error though because there may be attributes where it is not valid
					//to have a corresponding client side validation rule.
					//throw new NotImplementedException($"Type: {validationAttribute.GetType().FullName}");
				}

				validationBuilder.AppendLineWithTabIndent($".withMessage({message})", 4);
			}
		}

		protected override void WriteEnd(Type modelType, StringBuilder typeBuilder, StringBuilder validationBuilder)
		{
			//Only write the validation rules if some validation rules have been generated
			if (validationBuilder?.Length > 0)
			{
				typeBuilder.AppendLine();

				//Write out the constructor
				typeBuilder.AppendLineWithTabIndent("constructor()", 2)
					.AppendLineWithTabIndent("{", 2)
					.AppendLineWithTabIndent("ValidationRules", 3)
					.Append(validationBuilder.ToString())
					.AppendLineWithTabIndent(".on(this)", 4)
					.AppendLineWithTabIndent("}", 2)
					.AppendLine();
			}

			base.WriteEnd(modelType, typeBuilder, validationBuilder);
		}
		#endregion
	}
}