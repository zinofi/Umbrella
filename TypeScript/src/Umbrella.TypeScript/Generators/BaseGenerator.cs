using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Umbrella.TypeScript.Generators.Abstractions;

namespace Umbrella.TypeScript.Generators
{
	/// <summary>
	/// Serves as the base class for the <see cref="BaseInterfaceGenerator"/> and the <see cref="BaseClassGenerator" />.
	/// </summary>
	/// <seealso cref="IGenerator" />
	public abstract class BaseGenerator : IGenerator
	{
		/// <inheritdoc />
		public abstract TypeScriptOutputModelType OutputModelType { get; }

		/// <summary>
		/// Gets a value indicating whether this generator supports outputting validation rules.
		/// </summary>
		protected abstract bool SupportsValidationRules { get; }

		/// <summary>
		/// Gets or sets a value indicating whether strict null checks are being used.
		/// </summary>
		protected bool StrictNullChecks { get; set; }

		/// <summary>
		/// Gets or sets the property mode.
		/// </summary>
		protected TypeScriptPropertyMode PropertyMode { get; set; }

		/// <inheritdoc />
		public virtual string Generate(Type modelType, bool generateValidationRules, bool strictNullChecks, TypeScriptPropertyMode propertyMode)
		{
			StrictNullChecks = strictNullChecks;
			PropertyMode = propertyMode;

			var typeBuilder = new StringBuilder();

			//Only create an instance if validation rules need to be generated
			StringBuilder? validationBuilder = generateValidationRules ? new StringBuilder() : null;

			//Write the start of the type
			WriteStart(modelType, typeBuilder);

			//Write all properties. This may or may not generate validation rules.
			WriteAllProperties(modelType, GetModelProperties(modelType), typeBuilder, validationBuilder);

			//Write the end of the type. We pass in the validationBuilder here so that the content
			//of the validationBuilder can be written to the type in a way that is specific to the generator.
			WriteEnd(modelType, typeBuilder, validationBuilder);

			return typeBuilder.ToString();
		}

		/// <summary>
		/// Writes the start of the generated type.
		/// </summary>
		/// <param name="modelType">Type of the model.</param>
		/// <param name="builder">The builder.</param>
		protected abstract void WriteStart(Type modelType, StringBuilder builder);

		/// <summary>
		/// Writes all properties.
		/// </summary>
		/// <param name="modelType">Type of the model.</param>
		/// <param name="properties">The properties.</param>
		/// <param name="typeBuilder">The type builder.</param>
		/// <param name="validationBuilder">The validation builder.</param>
		protected virtual void WriteAllProperties(Type modelType, IEnumerable<PropertyInfo> properties, StringBuilder typeBuilder, StringBuilder? validationBuilder)
		{
			foreach (PropertyInfo pi in properties)
			{
				var typeOverrideAttribute = pi.GetCustomAttribute<TypeScriptOverrideAttribute>();

				Type propertyType = typeOverrideAttribute is not null
					? typeOverrideAttribute.TypeOverride
					: pi.PropertyType;

				TypeScriptMemberInfo tsInfo = TypeScriptUtility.GetTypeScriptMemberInfo(modelType, propertyType, pi, OutputModelType, StrictNullChecks, PropertyMode);

				WriteProperty(pi, tsInfo, typeBuilder);

				//We are generating the validation rules here so that this work can be done in the same step
				//as the work to generate the property itself.
				if (validationBuilder is not null)
					WriteValidationRules(pi, tsInfo, validationBuilder);
			}
		}

		/// <summary>
		/// Writes the property.
		/// </summary>
		/// <param name="pi">The pi.</param>
		/// <param name="tsInfo">The ts information.</param>
		/// <param name="typeBuilder">The type builder.</param>
		protected abstract void WriteProperty(PropertyInfo pi, TypeScriptMemberInfo tsInfo, StringBuilder typeBuilder);

		/// <summary>
		/// Writes the validation rules.
		/// </summary>
		/// <param name="propertyInfo">The property information.</param>
		/// <param name="tsInfo">The ts information.</param>
		/// <param name="validationBuilder">The validation builder.</param>
		/// <exception cref="NotImplementedException">This generator has been marked as supporting validation rules but doesn't implement this method.</exception>
		protected virtual void WriteValidationRules(PropertyInfo propertyInfo, TypeScriptMemberInfo tsInfo, StringBuilder? validationBuilder)
		{
			//If the generator implementation supports validation rules but they haven't been implmented
			//throw an exception to indicate this.
			if (SupportsValidationRules)
				throw new NotImplementedException("This generator has been marked as supporting validation rules but doesn't implement this method.");
		}

		/// <summary>
		/// Writes the end of the type.
		/// </summary>
		/// <param name="modelType">Type of the model.</param>
		/// <param name="typeBuilder">The type builder.</param>
		/// <param name="validationBuilder">The validation builder.</param>
		protected virtual void WriteEnd(Type modelType, StringBuilder typeBuilder, StringBuilder? validationBuilder) => typeBuilder.AppendLine("\t}");

		/// <summary>
		/// Gets the model properties.
		/// </summary>
		/// <param name="modelType">Type of the model.</param>
		/// <returns>The properties on the model.</returns>
		protected IEnumerable<PropertyInfo> GetModelProperties(Type modelType) => modelType.GetProperties().Where(x => x.GetCustomAttribute<TypeScriptIgnoreAttribute>() is null).OrderBy(x => x.Name);
	}
}