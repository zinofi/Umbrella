using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Umbrella.DataAnnotations;
using Umbrella.Utilities.Extensions;

namespace Umbrella.TypeScript.Generators
{
	/// <summary>
	/// A TypeScript generator implementation for generating Knockout classes.
	/// </summary>
	/// <seealso cref="Umbrella.TypeScript.Generators.BaseClassGenerator" />
	public class KnockoutClassGenerator : BaseClassGenerator
	{
		#region Private Constants
		/// <summary>
		/// This is the same regex used inside the implementation of the EmailAddressAttribute. Found this by checking the Microsoft Reference Source.
		/// This string has been double escaped so it is output in a JavaScript friendly format.
		/// </summary>
		private const string RegexEmail = "new RegExp(\"^((([a-z]|\\\\d|[!#\\\\$%&'\\\\*\\\\+\\\\-\\\\/=\\\\?\\\\^_`{\\\\|}~]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])+(\\\\.([a-z]|\\\\d|[!#\\\\$%&'\\\\*\\\\+\\\\-\\\\/=\\\\?\\\\^_`{\\\\|}~]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])+)*)|((\\\\x22)((((\\\\x20|\\\\x09)*(\\\\x0d\\\\x0a))?(\\\\x20|\\\\x09)+)?(([\\\\x01-\\\\x08\\\\x0b\\\\x0c\\\\x0e-\\\\x1f\\\\x7f]|\\\\x21|[\\\\x23-\\\\x5b]|[\\\\x5d-\\\\x7e]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])|(\\\\\\\\([\\\\x01-\\\\x09\\\\x0b\\\\x0c\\\\x0d-\\\\x7f]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF]))))*(((\\\\x20|\\\\x09)*(\\\\x0d\\\\x0a))?(\\\\x20|\\\\x09)+)?(\\\\x22)))@((([a-z]|\\\\d|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])|(([a-z]|\\\\d|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])([a-z]|\\\\d|-|\\\\.|_|~|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])*([a-z]|\\\\d|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])))\\\\.)+(([a-z]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])|(([a-z]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])([a-z]|\\\\d|-|\\\\.|_|~|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])*([a-z]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])))\\\\.?$\", \"i\")";
		#endregion

		private readonly bool _useDecorators;

		#region Overridden Properties
		/// <inheritdoc />
		public override TypeScriptOutputModelType OutputModelType => TypeScriptOutputModelType.KnockoutClass;

		/// <inheritdoc />
		protected override bool SupportsValidationRules => true;

		/// <inheritdoc />
		protected override TypeScriptOutputModelType InterfaceModelType => TypeScriptOutputModelType.KnockoutInterface;
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="KnockoutClassGenerator"/> class.
		/// </summary>
		/// <param name="useDecorators">if set to <c>true</c>, uses TypeScript decorators.</param>
		public KnockoutClassGenerator(bool useDecorators)
		{
			_useDecorators = useDecorators;
		}

		#region Overridden Methods
		/// <inheritdoc />
		protected override void WriteProperty(PropertyInfo pi, TypeScriptMemberInfo tsInfo, StringBuilder builder)
		{
			if (!string.IsNullOrEmpty(tsInfo.TypeName))
			{
				string strInitialOutputValue = PropertyMode switch
				{
					TypeScriptPropertyMode.Null => "null",
					TypeScriptPropertyMode.Model => tsInfo.InitialOutputValue ?? "null",
					_ => "",
				};

				string strStrictNullCheck = StrictNullChecks && (tsInfo.IsNullable || PropertyMode == TypeScriptPropertyMode.Null) ? " | null" : "";

				var formatString = new StringBuilder();

				if (_useDecorators)
				{
					formatString.AppendLineWithTabIndent("@observable({ expose: true })", 2);

					StringBuilder? coreValidationBuilder = CreateValidationExtendItems(pi, 3);

					if (coreValidationBuilder?.Length > 0)
					{
						formatString.AppendLineWithTabIndent("@extend({", 2);
						formatString.Append(coreValidationBuilder);
						formatString.AppendLineWithTabIndent("})", 2);
					}

					formatString.AppendLineWithTabIndent($"public {tsInfo.Name}: {tsInfo.TypeName}{strStrictNullCheck} = {strInitialOutputValue};", 2);
				}
				else
				{
					formatString.Append($"\t\t{tsInfo.Name}: ");

					if (tsInfo.TypeName?.EndsWith("[]") is true)
					{
						tsInfo.TypeName = tsInfo.TypeName.TrimEnd('[', ']');
						formatString.Append($"KnockoutObservableArray<{tsInfo.TypeName}{strStrictNullCheck}> = ko.observableArray<{tsInfo.TypeName}{strStrictNullCheck}>({strInitialOutputValue});");
					}
					else
					{
						formatString.Append($"KnockoutObservable<{tsInfo.TypeName}{strStrictNullCheck}> = ko.observable<{tsInfo.TypeName}{strStrictNullCheck}>({strInitialOutputValue});");
					}
				}

				builder.AppendLine(formatString.ToString());
			}
		}

		/// <inheritdoc />
		protected override void WriteValidationRules(PropertyInfo propertyInfo, TypeScriptMemberInfo tsInfo, StringBuilder? validationBuilder)
		{
			StringBuilder? ctorExtendBuilder = CreateConstructorValidationRules(propertyInfo);

			string thisVariable = "this";
			string exposePrefix = "";

			if (_useDecorators)
			{
				if (ctorExtendBuilder?.Length > 0)
				{
					exposePrefix = "_";
					thisVariable = "(<any>this)";
				}
				else
				{
					return;
				}
			}

			var coreBuilder = !_useDecorators ? CreateValidationExtendItems(propertyInfo) : null;

			if (validationBuilder != null && (coreBuilder?.Length > 0 || ctorExtendBuilder?.Length > 0))
			{
				if (_useDecorators)
				{
					validationBuilder
						.AppendLineWithTabIndent($"{thisVariable}.{exposePrefix}{tsInfo.Name.ToCamelCaseInvariant()}.extend({{", 3);
				}
				else
				{
					validationBuilder
						.AppendLineWithTabIndent($"{thisVariable}.{exposePrefix}{tsInfo.Name.ToCamelCaseInvariant()} = {thisVariable}.{exposePrefix}{tsInfo.Name.ToCamelCaseInvariant()}.extend({{", 3);
				}

				validationBuilder
					.Append(coreBuilder)
					.Append(ctorExtendBuilder)
					.AppendLineWithTabIndent("});", 3)
					.AppendLine();
			}
		}

		private StringBuilder? CreateValidationExtendItems(PropertyInfo propertyInfo, int indent = 4)
		{
			//Get all types that are either of type ValidationAttribute or derive from it
			//However, specifically exclude instances of type DataTypeAttribute
			var lstValidationAttribute = propertyInfo.GetCustomAttributes<ValidationAttribute>().Where(x => x.GetType() != typeof(DataTypeAttribute)).ToList();

			if (lstValidationAttribute.Count is 0)
				return null;

			var validationBuilder = new StringBuilder();

			for (int i = 0; i < lstValidationAttribute.Count; i++)
			{
				var validationAttribute = lstValidationAttribute[i];

				string message = $"\"{validationAttribute.FormatErrorMessage(propertyInfo.Name)}\"";

				switch (validationAttribute)
				{
					case RequiredAttribute attr:
					case RequiredNonEmptyCollectionAttribute attrCol: // TODO: Knockout should be fine with this.
						validationBuilder.AppendLineWithTabIndent($"required: {{ params: true, message: {message} }},", indent);
						break;
					case CompareAttribute attr:
						string otherPropertyName = attr.OtherProperty.ToCamelCaseInvariant();
						validationBuilder.AppendLineWithTabIndent($"equal: {{ params: this.{otherPropertyName}, message: {message} }},", indent);
						break;
					case EmailAddressAttribute attr:
						validationBuilder.AppendLineWithTabIndent($"pattern: {{ params: {RegexEmail}, message: {message} }},", indent);
						break;
					case MinLengthAttribute attr:
						validationBuilder.AppendLineWithTabIndent($"minLength: {{ params: {attr.Length}, message: {message} }},", indent);
						break;
					case MaxLengthAttribute attr:
						validationBuilder.AppendLineWithTabIndent($"maxLength: {{ params: {attr.Length}, message: {message} }},", indent);
						break;
					case RangeAttribute attr:
						validationBuilder.AppendLineWithTabIndent($"min: {{ params: {attr.Minimum}, message: {message} }},", indent);
						validationBuilder.AppendLineWithTabIndent($"max: {{ params: {attr.Maximum}, message: {message} }},", indent);
						break;
					case RegularExpressionAttribute attr:
						validationBuilder.AppendLineWithTabIndent($"pattern: {{ params: /{attr.Pattern}/, message: {message} }},", indent);
						break;
					case StringLengthAttribute attr:
						if (attr.MinimumLength > 0)
							validationBuilder.AppendLineWithTabIndent($"minLength: {{ params: {attr.MinimumLength}, message: {message} }},", indent);

						validationBuilder.AppendLineWithTabIndent($"maxLength: {{ params: {attr.MaximumLength}, message: {message} }},", indent);
						break;
					case RequiredTrueAttribute attr:
						validationBuilder.AppendLineWithTabIndent($"validation: {{ params: true, validator: (val, otherVal) => val === otherVal, message: {message} }},", indent);
						break;
				}
			}

			return validationBuilder;
		}

		private StringBuilder? CreateConstructorValidationRules(PropertyInfo propertyInfo, int indent = 4)
		{
			string GetDependentPropertyName(string dependentProperty)
			{
				string propertyName = dependentProperty.Contains(".")
					? string.Join("?.", dependentProperty.Split('.').Select(x => x.ToCamelCaseInvariant()))
					: dependentProperty.ToCamelCaseInvariant();

				// If we are not using decorators we need to access the value using a method call.
				if (!_useDecorators)
					propertyName += "()";

				return propertyName;
			}

			// Get all types that are either of type ValidationAttribute or derive from it
			// However, specifically exclude instances of type DataTypeAttribute
			var lstValidationAttribute = propertyInfo.GetCustomAttributes<ValidationAttribute>().Where(x => x.GetType() != typeof(DataTypeAttribute)).ToList();

			if (lstValidationAttribute.Count is 0)
				return null;

			var validationBuilder = new StringBuilder();

			for (int i = 0; i < lstValidationAttribute.Count; i++)
			{
				var validationAttribute = lstValidationAttribute[i];

				string message = $"\"{validationAttribute.FormatErrorMessage(propertyInfo.Name)}\"";

				switch (validationAttribute)
				{
					case RequiredIfAttribute attr:
						{
							string @operator = attr.Operator switch
							{
								Operator.EqualTo => "===",
								Operator.GreaterThan => ">",
								Operator.GreaterThanOrEqualTo => ">=",
								Operator.LessThan => "<",
								Operator.LessThanOrEqualTo => "<=",
								Operator.NotEqualTo => "!==",
								Operator.NotRegExMatch => throw new NotImplementedException(),
								Operator.RegExMatch => throw new NotImplementedException(),
								_ => throw new NotSupportedException()
							};

							string otherValue = attr.DependentValue switch
							{
								bool b when b => "true",
								bool b when !b => "false",
								string s => $"'{s}'",
								Enum e => $"{e.GetType().FullName}.{e}",
								_ => attr.DependentValue.ToString()
							};

							string dependentPropertyName = GetDependentPropertyName(attr.DependentProperty);

							validationBuilder.AppendLineWithTabIndent($"required: {{ onlyIf: () => this.{dependentPropertyName} {@operator} {otherValue}, message: {message} }},", indent);
						}
						break;
					case RequiredIfEmptyAttribute attr:
						{
							string dependentPropertyName = GetDependentPropertyName(attr.DependentProperty);

							validationBuilder.AppendLineWithTabIndent($"required: {{ onlyIf: () => this.{dependentPropertyName} === undefined || this.{dependentPropertyName} === null || (typeof this.{dependentPropertyName} === \"number\" && isNaN(this.{dependentPropertyName}!)) || (typeof this.{dependentPropertyName} === \"string\" && (this.{dependentPropertyName}! as any).trim().length === 0), message: {message} }},", indent);
						}
						break;
					case RequiredIfNotEmptyAttribute attr:
						{
							string dependentPropertyName = GetDependentPropertyName(attr.DependentProperty);

							validationBuilder.AppendLineWithTabIndent($"required: {{ onlyIf: () => this.{dependentPropertyName} !== undefined && this.{dependentPropertyName} !== null && !(typeof this.{dependentPropertyName} === \"number\" && !isNaN(this.{dependentPropertyName}!)) && !(typeof this.{dependentPropertyName} === \"string\" && (this.{dependentPropertyName}! as any).trim().length > 0), message: {message} }},", indent);
						}
						break;
				}
			}

			return validationBuilder;
		}

		/// <inheritdoc />
		protected override void WriteEnd(Type modelType, StringBuilder typeBuilder, StringBuilder? validationBuilder)
		{
			// Only write the validation rules if some validation rules have been generated
			// and we are not using decorators
			if (!_useDecorators && validationBuilder?.Length > 0)
			{
				typeBuilder.AppendLine();

				//Write out the constructor
				typeBuilder.AppendLineWithTabIndent("constructor()", 2)
					.AppendLineWithTabIndent("{", 2)
					.Append(validationBuilder.ToString())
					.AppendLineWithTabIndent("}", 2)
					.AppendLine();
			}

			base.WriteEnd(modelType, typeBuilder, validationBuilder);
		}
		#endregion
	}
}