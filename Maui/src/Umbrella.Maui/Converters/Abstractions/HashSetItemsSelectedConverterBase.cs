using System.Globalization;
using CommunityToolkit.Diagnostics;
using Microsoft.Maui.Controls;

namespace Umbrella.Maui.Converters.Abstractions;

// TODO: Not sure why this exists with the capability to choose a model type? What value could that give?? Seems like an overcomplication.
// Maybe rename this to EnumHashSetSelectedItemsConverter, remove TModel and simplify.
// Only used on Temp365 by the DentalPracticeLocationSpecialismTypeEnumSelectedConverter and that just uses the enum for the model and enum types anyway.

/// <summary>
/// A base class which can be used to create converters that allow items in a list of enums to be selected and unselected
/// based on a list of selected items. The act of selecting and deselecting an enum in the list will cause a model of type <typeparamref name="TModel"/>
/// to be added or removed to/from the <see cref="HashSet{TModel}"/> set as the value of the attached property <see cref="SelectedItemsProperty"/>.
/// </summary>
/// <typeparam name="TConverter">The type of the converter.</typeparam>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TEnum">The type of the enum.</typeparam>
/// <seealso cref="IValueConverter" />
public abstract class HashSetItemsSelectedConverterBase<TConverter, TModel, TEnum> : IValueConverter
	where TEnum : struct, Enum
{
	/// <summary>
	/// The selected items attached property
	/// </summary>
	public static readonly BindableProperty SelectedItemsProperty = BindableProperty.CreateAttached("SelectedItems", typeof(HashSet<TModel>), typeof(TConverter), null);

	/// <summary>
	/// Gets the selected items.
	/// </summary>
	/// <param name="target">The target.</param>
	/// <returns>The selected items.</returns>
	public static HashSet<TModel> GetSelectedItems(BindableObject target)
	{
		Guard.IsNotNull(target);

		return (HashSet<TModel>)target.GetValue(SelectedItemsProperty);
	}

	/// <summary>
	/// Sets the selected items.
	/// </summary>
	/// <param name="target">The target.</param>
	/// <param name="value">The value.</param>
	public static void SetSelectedItems(BindableObject target, HashSet<TModel> value)
	{
		Guard.IsNotNull(target);

		target.SetValue(SelectedItemsProperty, value);
	}

	/// <summary>
	/// Gets the enum selector.
	/// </summary>
	protected abstract Func<TModel, TEnum> EnumSelector { get; }

	/// <inheritdoc />
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> value is HashSet<TModel> selectedItems && selectedItems.Any(x => EnumSelector(x).Equals(HashSetItemsSelectedConverterBase<TConverter, TModel, TEnum>.GetParameterValue(parameter)));

	/// <inheritdoc />
	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (parameter is not BindableObject bindableObject)
			throw new InvalidOperationException("The parameter must be a bindable object.");

		HashSet<TModel> lstExistingValue = GetSelectedItems(bindableObject);

		if (value is bool selected)
		{
			TEnum type = HashSetItemsSelectedConverterBase<TConverter, TModel, TEnum>.GetParameterValue(parameter);

			if (selected)
			{
				_ = lstExistingValue.Add(CreateModel(type));
			}
			else
			{
				_ = lstExistingValue.RemoveWhere(x => EnumSelector(x).Equals(type));
			}
		}

		return lstExistingValue;
	}

	/// <summary>
	/// Creates the model.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>The model.</returns>
	protected abstract TModel CreateModel(TEnum value);

	private static TEnum GetParameterValue(object? parameter) => parameter switch
	{
		BindableObject bindable when bindable.BindingContext is TEnum t => t,
		_ => throw new NotSupportedException("The parameter cannot be converted.")
	};
}