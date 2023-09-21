using System.Linq.Expressions;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Sorting;
using Umbrella.Utilities.TypeConverters;

namespace Umbrella.AspNetCore.Blazor.Components.Grid;

// 1. Try and change the filter value from a string to a generic type. When sending to the server, we'll need to call .ToString on it.
// What impact will this have? For primitive types, this should work as it currently does.
// For custom types, the user will have to override the .ToString method.
// For custom types, the filter value should not be directly editable though, e.g. for a date range, it needs to be selected from a modal, or combo of fields which
// is readonly.
// For autocomplete, the selected value needs to be used which might not be a string, it might be an Id.
// For add-on button, it might be a complex object again. Will we break anything doing this? Should be ok on Reach where it's used by ensuring types default to strings.
// We will also need to create an interface that can be used to allow instances of columns to be passed around without running into problems with generic type parameters
// not being know.

// 2. After doing that, we need to create an overloaded version of this class with suitable defaults for the autocomplete type parameters.
// Although these can live on the base class, they need to be defaulted.

public interface IUmbrellaColumnDefinition<TItem>
{
	IReadOnlyDictionary<string, object> AdditionalAttributes { get; }
	string? AddOnButtonCssClass { get; set; }
	string? AddOnButtonIconCssClass { get; set; }
	string? AddOnButtonText { get; set; }
	Func<string, Task<IEnumerable<object>>>? AutoCompleteSearchMethod { get; set; }
	string ColumnWidthCssClass { get; }
	SortDirection? Direction { get; set; }
	string DirectionCssClass { get; }
	UmbrellaColumnDisplayMode DisplayMode { get; }
	bool Filterable { get; }
	UmbrellaColumnFilterType FilterControlType { get; }
	FilterType FilterMatchType { get; }
	string? FilterMemberPathOverride { get; }
	Func<object, string>? FilterOptionDisplayNameSelector { get; }
	IReadOnlyCollection<object>? FilterOptions { get; }
	UmbrellaColumnFilterOptionsType? FilterOptionsType { get; }
	string? FilterValue { get; set; }
	string? Heading { get; }
	string? NullableEnumOption { get; set; }
	Func<string?, ValueTask<string?>>? OnAddOnButtonClickedAsync { get; set; }
	int? PercentageWidth { get; }
	string? PropertyName { get; }
	string? ShortHeading { get; }
	bool Sortable { get; }
	string? SorterMemberPathOverride { get; }
	string GetFilterOptionDisplayName(object option);
	Type FilterValueType { get; }
}

/// <summary>
/// Defines a column displayed using the <see cref="UmbrellaGrid{TItem}"/> component.
/// </summary>
public record UmbrellaColumnDefinition<TItem, TValue> : IUmbrellaColumnDefinition<TItem>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaColumnDefinition{TItem, TValue}"/> class.
	/// </summary>
	/// <param name="heading">The heading.</param>
	/// <param name="shortHeading">The short heading.</param>
	/// <param name="percentageWidth">Width of the percentage.</param>
	/// <param name="sortable">if set to <c>true</c> allows the column to be sortable.</param>
	/// <param name="filterable">if set to <c>true</c> allows the column to be filterable.</param>
	/// <param name="filterOptions">The filter options.</param>
	/// <param name="filterOptionDisplayNameSelector">The filter option display name selector.</param>
	/// <param name="additionalAttributes">The additional attributes.</param>
	/// <param name="filterControlType">Type of the filter control.</param>
	/// <param name="filterMatchType">Type of the filter match.</param>
	/// <param name="filterOptionsType">Type of the filter options.</param>
	/// <param name="property">The property selector.</param>
	/// <param name="filterMemberPathOverride">The filter member override.</param>
	/// <param name="sorterMemberPathOverride">The sorter member override.</param>
	/// <param name="displayMode">The display mode.</param>
	/// <param name="nullableEnumOption">The nullable enum option.</param>
	/// <param name="onAddOnButtonClickedAsync">The delegate invoked when the text addon button is clicked.</param>
	/// <param name="addOnButtonCssClass">The CSS class for the add-on button.</param>
	/// <param name="addOnButtonText">The text for the add-on button.</param>
	/// <param name="addOnButtonIconCssClass">The CSS class for the icon displayed on the add-on button.</param>
	public UmbrellaColumnDefinition(
		string? heading,
		string? shortHeading,
		int? percentageWidth,
		bool sortable,
		bool filterable,
		IReadOnlyCollection<object>? filterOptions,
		Func<object, string>? filterOptionDisplayNameSelector,
		IReadOnlyDictionary<string, object> additionalAttributes,
		UmbrellaColumnFilterType filterControlType,
		FilterType filterMatchType,
		UmbrellaColumnFilterOptionsType? filterOptionsType,
		Expression<Func<TItem, TValue?>>? property,
		string? filterMemberPathOverride,
		string? sorterMemberPathOverride,
		UmbrellaColumnDisplayMode displayMode,
		string? nullableEnumOption,
		Func<string?, ValueTask<string?>>? onAddOnButtonClickedAsync,
		string? addOnButtonCssClass,
		string? addOnButtonText,
		string? addOnButtonIconCssClass)
	{
		Heading = heading;
		ShortHeading = shortHeading;
		PercentageWidth = percentageWidth;
		Sortable = sortable;
		Filterable = filterable;
		FilterOptions = filterOptions;
		FilterOptionDisplayNameSelector = filterOptionDisplayNameSelector;
		AdditionalAttributes = additionalAttributes;
		FilterControlType = filterControlType;
		FilterMatchType = filterMatchType;
		FilterOptionsType = filterOptionsType;
		Property = property;
		PropertyName = property?.GetMemberName();
		FilterMemberPathOverride = filterMemberPathOverride;
		SorterMemberPathOverride = sorterMemberPathOverride;
		DisplayMode = displayMode;
		NullableEnumOption = nullableEnumOption;
		OnAddOnButtonClickedAsync = onAddOnButtonClickedAsync;
		AddOnButtonCssClass = addOnButtonCssClass;
		AddOnButtonText = addOnButtonText;
		AddOnButtonIconCssClass = addOnButtonIconCssClass;

		if (FilterOptionsType is UmbrellaColumnFilterOptionsType.Enum)
		{
			FilterControlType = UmbrellaColumnFilterType.Options;
			FilterMatchType = FilterType.Equal;
		}
		else if (FilterOptionsType is UmbrellaColumnFilterOptionsType.Boolean)
		{
			FilterControlType = UmbrellaColumnFilterType.Options;
			FilterMatchType = FilterType.Equal;
			FilterOptions = new[] { "Yes", "No" };
			NullableEnumOption = null;
		}
		else if (FilterControlType is UmbrellaColumnFilterType.Options || FilterOptions is not null)
		{
			FilterControlType = UmbrellaColumnFilterType.Options;
			FilterOptionsType = UmbrellaColumnFilterOptionsType.String;
			FilterMatchType = FilterType.Equal;
			NullableEnumOption = null;
		}

		if (Property is not null)
		{
			if (string.IsNullOrEmpty(Heading))
			{
				Heading = Property.GetDisplayText();
			}

			if (string.IsNullOrEmpty(ShortHeading))
			{
				ShortHeading = Property.GetShortNameDisplayText() ?? Property.GetDisplayText();
			}
		}

		FilterValueType = typeof(TValue);
	}

	/// <summary>
	/// Gets the property selector.
	/// </summary>
	public Expression<Func<TItem, TValue?>>? Property { get; }

	/// <summary>
	/// Gets or sets the name of the property.
	/// </summary>
	public string? PropertyName { get; }

	/// <summary>
	/// Gets or sets the property path override used as the <see cref="FilterExpressionDescriptor.MemberPath"/> property value when
	/// creating filters.
	/// </summary>
	/// <remarks>
	/// If this value is <see langword="null"/>, the <see cref="Property"/> will be used.
	/// </remarks>
	public string? FilterMemberPathOverride { get; }

	/// <summary>
	/// Gets or sets the property path override used as the value <see cref="SortExpressionDescriptor.MemberPath"/> property value when
	/// creating sorters.
	/// </summary>
	/// <remarks>
	/// If this value is <see langword="null"/>, the <see cref="Property"/> will be used.
	/// </remarks>
	public string? SorterMemberPathOverride { get; }

	/// <summary>
	/// Gets the display mode.
	/// </summary>
	public UmbrellaColumnDisplayMode DisplayMode { get; }

	/// <summary>
	/// Gets the column heading display text.
	/// </summary>
	public string? Heading { get; }

	/// <summary>
	/// Gets the column short heading display text.
	/// </summary>
	public string? ShortHeading { get; }

	/// <summary>
	/// Gets the percentage width of the column.
	/// </summary>
	public int? PercentageWidth { get; }

	/// <summary>
	/// Gets a value specifying whether or not the column is sortable.
	/// </summary>
	public bool Sortable { get; }

	/// <summary>
	/// Gets a value specifying whether or not the column is filterable.
	/// </summary>
	public bool Filterable { get; }

	/// <summary>
	/// Gets a value specifying the type of control that is rendered to allow filtering, if enabled using the <see cref="Filterable"/> property.
	/// </summary>
	public UmbrellaColumnFilterType FilterControlType { get; }

	/// <summary>
	/// Gets a value specifying how the <see cref="FilterValue"/> will be matched against the data in the column.
	/// </summary>
	public FilterType FilterMatchType { get; }

	/// <summary>
	/// Gets the filter options displayed to the user.
	/// </summary>
	public IReadOnlyCollection<object>? FilterOptions { get; }

	/// <summary>
	/// Specifies the type of the <see cref="FilterOptions"/> displayed to the user.
	/// </summary>
	public UmbrellaColumnFilterOptionsType? FilterOptionsType { get; }

	/// <summary>
	/// Gets a delegate used to convert a filter option to a friendly string for displaying to the user.
	/// </summary>
	public Func<object, string>? FilterOptionDisplayNameSelector { get; }

	/// <summary>
	/// Gets a collection of the unmatched parameter values specified on the column component that this instance is associated with.
	/// </summary>
	public IReadOnlyDictionary<string, object> AdditionalAttributes { get; }

	/// <summary>
	/// Gets or sets the value used to filter the column.
	/// </summary>
	public string? FilterValue { get; set; }

	/// <summary>
	/// Gets or sets the nullable enum option.
	/// </summary>
	/// <remarks>
	/// If a value is provided, an option will be shown when the <see cref="FilterOptionsType"/> is set to <see cref="UmbrellaColumnFilterOptionsType.Enum"/>
	/// which will show a new option after the <c>Any</c> option with an explcit value of <see langword="null" /> with the value specified for this property
	/// value displayed as the text in the dropdown for the option.
	/// </remarks>
	public string? NullableEnumOption { get; set; }

	/// <summary>
	/// Gets or sets the addon button delegate which will be invoked when the add-on button is clicked when the <see cref="FilterControlType"/>
	/// is set to <see cref="UmbrellaColumnFilterType.TextAddOnButton"/>.
	/// </summary>
	/// <remarks>
	/// The delegate must accept a string parameter which will be the value of the current filter and return a string, which is the new filter value
	/// to set the text box's content to, wrapped in a <see cref="ValueTask"/>.
	/// </remarks>
	public Func<string?, ValueTask<string?>>? OnAddOnButtonClickedAsync { get; set; }

	/// <summary>
	/// Gets or sets the add on button CSS class.
	/// </summary>
	public string? AddOnButtonCssClass { get; set; }

	/// <summary>
	/// Gets or sets the add on button text.
	/// </summary>
	public string? AddOnButtonText { get; set; }

	/// <summary>
	/// Gets or sets the add on button icon CSS class.
	/// </summary>
	public string? AddOnButtonIconCssClass { get; set; }

	public Func<string, Task<IEnumerable<object>>>? AutoCompleteSearchMethod { get; set; }

	/// <summary>
	/// Gets or sets the sort direction.
	/// </summary>
	public SortDirection? Direction { get; set; }

	/// <summary>
	/// Gets the sort direction CSS class based on the current value of the <see cref="Direction"/> property.
	/// </summary>
	public string DirectionCssClass => Direction switch
	{
		SortDirection.Ascending => "fas fa-sort-up",
		SortDirection.Descending => "fas fa-sort-down",
		_ => "fas fa-sort",
	};

	/// <summary>
	/// Gets the CSS class for the width of the column based on the value of the <see cref="PercentageWidth"/> property.
	/// </summary>
	public string ColumnWidthCssClass => PercentageWidth.HasValue ? $"u-grid__column--{PercentageWidth}" : "u-grid__column--auto";

	/// <summary>
	/// Gets the type of the filter value.
	/// </summary>
	/// <remarks>
	/// This is the <see cref="Type"/> of the <typeparamref name="TValue"/> generic type parameter.
	/// </remarks>
	public Type FilterValueType { get; }

	public TValue? TypedFilterValue
	{
		get => FilterValue is not null ? GenericTypeConverterHelper.Convert<TValue>(FilterValue) : default;
		set => FilterValue = value is not null ? value.ToString() : default;
	}

	/// <summary>
	/// Gets a friendly display name for a specified filter <paramref name="option"/>.
	/// </summary>
	/// <param name="option">The filter option.</param>
	/// <returns>The friendly display name.</returns>
	public string GetFilterOptionDisplayName(object option)
	{
		if (FilterOptionDisplayNameSelector is null)
		{
			if (option is string strOption)
			{
				var lstChar = new List<char>();

				for (int i = 0; i < strOption.Length; i++)
				{
					lstChar.Add(strOption[i]);

					if (i + 1 < strOption.Length && strOption[i + 1] == char.ToUpperInvariant(strOption[i + 1]))
						lstChar.Add(' ');
				}

				return string.Join("", lstChar);
			}
			else if (option is Enum enumOption)
			{
				return enumOption.ToDisplayString();
			}

			return option.ToString() ?? throw new UmbrellaBlazorException("The display name for the option must have a value.");
		}

		return FilterOptionDisplayNameSelector(option);
	}
}