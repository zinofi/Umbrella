using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Converters.Abstractions;

public abstract class HashSetItemsSelectedConverterBase<TConverter, TModel, TEnum> : IValueConverter
	where TEnum : struct, Enum
{
	public static readonly BindableProperty SelectedItemsProperty = BindableProperty.CreateAttached("SelectedItems", typeof(HashSet<TModel>), typeof(TConverter), null);

	public static HashSet<TModel> GetSelectedItems(BindableObject target) => (HashSet<TModel>)target.GetValue(SelectedItemsProperty);
	public static void SetSelectedItems(BindableObject target, HashSet<TModel> value) => target.SetValue(SelectedItemsProperty, value);

	protected abstract Func<TModel, TEnum> EnumSelector { get; }

	/// <inheritdoc />
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		=> value is HashSet<TModel> selectedItems && selectedItems.Any(x => EnumSelector(x).Equals(GetParameterValue(parameter)));

	/// <inheritdoc />
	public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (!(parameter is BindableObject bindableObject))
			throw new Exception("The parameter must be a bindable object.");

		HashSet<TModel> lstExistingValue = GetSelectedItems(bindableObject);

		if (value is bool selected)
		{
			TEnum type = GetParameterValue(parameter);

			if (selected)
			{
				lstExistingValue.Add(CreateModel(type));
			}
			else
			{
				lstExistingValue.RemoveWhere(x => EnumSelector(x).Equals(type));
			}
		}

		return lstExistingValue;
	}

	protected abstract TModel CreateModel(TEnum value);

	private TEnum GetParameterValue(object parameter) => parameter switch
	{
		BindableObject bindable when bindable.BindingContext is TEnum t => t,
		_ => throw new Exception("The parameter cannot be converted.")
	};
}