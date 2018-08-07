using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;
using Umbrella.TypeScript.Generators.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Umbrella.TypeScript.Generators
{
    public class KnockoutClassGenerator : BaseClassGenerator
    {
        #region Private Constants
        /// <summary>
        /// This is the same regex used inside the implementation of the EmailAddressAttribute. Found this by checking the Microsoft Reference Source.
        /// This string has been double escaped so it is output in a JavaScript friendly format.
        /// </summary>
        private const string c_RegexEmail = "new RegExp(\"^((([a-z]|\\\\d|[!#\\\\$%&'\\\\*\\\\+\\\\-\\\\/=\\\\?\\\\^_`{\\\\|}~]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])+(\\\\.([a-z]|\\\\d|[!#\\\\$%&'\\\\*\\\\+\\\\-\\\\/=\\\\?\\\\^_`{\\\\|}~]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])+)*)|((\\\\x22)((((\\\\x20|\\\\x09)*(\\\\x0d\\\\x0a))?(\\\\x20|\\\\x09)+)?(([\\\\x01-\\\\x08\\\\x0b\\\\x0c\\\\x0e-\\\\x1f\\\\x7f]|\\\\x21|[\\\\x23-\\\\x5b]|[\\\\x5d-\\\\x7e]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])|(\\\\\\\\([\\\\x01-\\\\x09\\\\x0b\\\\x0c\\\\x0d-\\\\x7f]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF]))))*(((\\\\x20|\\\\x09)*(\\\\x0d\\\\x0a))?(\\\\x20|\\\\x09)+)?(\\\\x22)))@((([a-z]|\\\\d|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])|(([a-z]|\\\\d|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])([a-z]|\\\\d|-|\\\\.|_|~|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])*([a-z]|\\\\d|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])))\\\\.)+(([a-z]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])|(([a-z]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])([a-z]|\\\\d|-|\\\\.|_|~|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])*([a-z]|[\\\\u00A0-\\\\uD7FF\\\\uF900-\\\\uFDCF\\\\uFDF0-\\\\uFFEF])))\\\\.?$\", \"i\")";
        #endregion

        #region Overridden Properties
        public override TypeScriptOutputModelType OutputModelType => TypeScriptOutputModelType.KnockoutClass;
        protected override bool SupportsValidationRules => false;
        protected override TypeScriptOutputModelType InterfaceModelType => TypeScriptOutputModelType.KnockoutInterface;
        #endregion

        #region Overridden Methods
        protected override void WriteProperty(TypeScriptMemberInfo tsInfo, StringBuilder builder)
        {
            if (!string.IsNullOrEmpty(tsInfo.TypeName))
            {
                string strInitialOutputValue;

                switch (PropertyMode)
                {
                    default:
                    case TypeScriptPropertyMode.None:
                        strInitialOutputValue = "";
                        break;
                    case TypeScriptPropertyMode.Null:
                        strInitialOutputValue = "null";
                        break;
                    case TypeScriptPropertyMode.Model:
                        strInitialOutputValue = tsInfo.InitialOutputValue;
                        break;
                }

                string strStrictNullCheck = StrictNullChecks && (tsInfo.IsNullable || PropertyMode == TypeScriptPropertyMode.Null) ? " | null" : "";

                string formatString = "\t\t{0}: ";

                if (tsInfo.TypeName.EndsWith("[]"))
                {
                    formatString += "KnockoutObservableArray<{1}> = ko.observableArray<{1}>({2});";
                    tsInfo.TypeName = tsInfo.TypeName.TrimEnd('[', ']');
                }
                else
                {
                    formatString += "KnockoutObservable<{1}> = ko.observable<{1}>({2});";
                }

                builder.AppendLine(string.Format(formatString, tsInfo.Name, $"{tsInfo.TypeName}{strStrictNullCheck}", strInitialOutputValue));
            }
        }

        protected override void WriteValidationRules(PropertyInfo propertyInfo, TypeScriptMemberInfo tsInfo, StringBuilder validationBuilder)
        {
            //Get all types that are either of type ValidationAttribute or derive from it
            //However, specifically exclude instances of type DataTypeAttribute
            var lstValidationAttribute = propertyInfo.GetCustomAttributes<ValidationAttribute>().Where(x => x.GetType() != typeof(DataTypeAttribute)).ToList();

            if (lstValidationAttribute.Count == 0)
                return;

            validationBuilder.AppendLineWithTabIndent($"this.{tsInfo.Name.ToCamelCaseInvariant()} = this.{tsInfo.Name.ToCamelCaseInvariant()}.extend({{", 3);

            for (int i = 0; i < lstValidationAttribute.Count; i++)
            {
                var validationAttribute = lstValidationAttribute[i];

                string message = $"\"{validationAttribute.FormatErrorMessage(propertyInfo.Name)}\"";

                switch (validationAttribute)
                {
                    case RequiredAttribute attr:
                        validationBuilder.AppendLineWithTabIndent($"required: {{ params: true, message: {message} }},", 4);
                        break;
                    case CompareAttribute attr:
                        string otherPropertyName = attr.OtherProperty.ToCamelCaseInvariant();
                        validationBuilder.AppendLineWithTabIndent($"equal: {{ params: this.{otherPropertyName}, message: {message} }},", 4);
                        break;
                    case EmailAddressAttribute attr:
                        validationBuilder.AppendLineWithTabIndent($"pattern: {{ params: {c_RegexEmail}, message: {message} }},", 4);
                        break;
                    case MinLengthAttribute attr:
                        validationBuilder.AppendLineWithTabIndent($"minLength: {{ params: {attr.Length}, message: {message} }},", 4);
                        break;
                    case MaxLengthAttribute attr:
                        validationBuilder.AppendLineWithTabIndent($"maxLength: {{ params: {attr.Length}, message: {message} }},", 4);
                        break;
                    case RangeAttribute attr:
                        validationBuilder.AppendLineWithTabIndent($"min: {{ params: {attr.Minimum}, message: {message} }},", 4);
                        validationBuilder.AppendLineWithTabIndent($"max: {{ params: {attr.Maximum}, message: {message} }},", 4);
                        break;
                    case RegularExpressionAttribute attr:
                        validationBuilder.AppendLineWithTabIndent($"pattern: {{ params: /{attr.Pattern}/), message: {message} }},", 4);
                        break;
                    case StringLengthAttribute attr:
                        if (attr.MinimumLength > 0)
                            validationBuilder.AppendLineWithTabIndent($"minLength: {{ params: {attr.MinimumLength}, message: {message} }},", 4);

                        validationBuilder.AppendLineWithTabIndent($"maxLength: {{ params: {attr.MaximumLength}, message: {message} }},", 4);
                        break;
                }
            }

            validationBuilder.AppendLineWithTabIndent("});", 3)
                .AppendLine();
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
                    .Append(validationBuilder.ToString())
                    .AppendLineWithTabIndent("}", 2)
                    .AppendLine();
            }

            base.WriteEnd(modelType, typeBuilder, validationBuilder);
        }
        #endregion
    }
}
