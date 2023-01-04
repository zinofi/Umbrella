using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Umbrella.DataAnnotations.BaseClasses;

/// <summary>
/// Serves as the base class for contingent validation attributes.
/// </summary>
/// <seealso cref="ModelAwareValidationAttribute" />
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public abstract class ContingentValidationAttribute : ModelAwareValidationAttribute
{
	/// <summary>
	/// Gets the name of the dependent property.
	/// </summary>
	public string DependentProperty { get; private set; }

	/// <summary>
	/// Gets or sets a value indicating whether validation will succeed if either
	/// the value on which this attrubute has been applied is null or the value of
	/// the <see cref="DependentProperty"/> is null.
	/// </summary>
	public bool ReturnTrueOnEitherNull { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ContingentValidationAttribute"/> class.
	/// </summary>
	/// <param name="dependentProperty">The dependent property.</param>
	public ContingentValidationAttribute(string dependentProperty)
	{
		DependentProperty = dependentProperty;
	}

	/// <inheritdoc />
	public override string FormatErrorMessage(string name)
	{
		if (string.IsNullOrEmpty(ErrorMessageResourceName) && string.IsNullOrEmpty(ErrorMessage))
			ErrorMessage = DefaultErrorMessageFormat;

		return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, DependentProperty);
	}

	/// <inheritdoc />
	public override string DefaultErrorMessageFormat { get; } = "{0} is invalid due to {1}.";

	/// <summary>
	/// Gets the dependent property value.
	/// </summary>
	/// <param name="container">The container.</param>
	/// <returns>The value of the dependent property from the container.</returns>
	private object GetDependentPropertyValue(object container)
	{
		var currentType = container.GetType();
		object value = container;

		foreach (string propertyName in DependentProperty.Split('.'))
		{
			var property = currentType.GetProperty(propertyName);
			value = property.GetValue(value, null);
			currentType = property.PropertyType;
		}

		return value;
	}

	/// <inheritdoc />
	protected override IEnumerable<KeyValuePair<string, object>> GetClientValidationParameters()
		=> base.GetClientValidationParameters().Union(new[] { new KeyValuePair<string, object>("DependentProperty", DependentProperty) });

	/// <inheritdoc />
	public override bool IsValid(object value, object container) => IsValid(value, GetDependentPropertyValue(container), container);

	/// <summary>
	/// If the value of <paramref name="dependentValue" /> is such that it matches the value of the <see cref="DependentProperty"/> value on
	/// the <paramref name="container"/>, validation will take place on the supplied <paramref name="value"/>.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="dependentValue">The dependent value.</param>
	/// <param name="container">The container.</param>
	/// <returns>
	///   <c>true</c> if the specified value is valid; otherwise, <c>false</c>.
	/// </returns>
	public abstract bool IsValid(object value, object dependentValue, object container);
}