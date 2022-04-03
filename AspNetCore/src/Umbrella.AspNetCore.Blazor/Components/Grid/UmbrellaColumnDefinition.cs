using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.AspNetCore.Blazor.Components.Grid
{
	/// <summary>
	/// Defines a column displayed using the <see cref="UmbrellaGrid{TItem}"/> component.
	/// </summary>
	public class UmbrellaColumnDefinition
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaColumnDefinition"/> class.
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
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="filterMemberPathOverride">The filter member override.</param>
		/// <param name="sorterMemberPathOverride">The sorter member override.</param>
		/// <param name="displayMode">The display mode.</param>
		public UmbrellaColumnDefinition(
			string? heading,
			string? shortHeading,
			int? percentageWidth,
			bool sortable,
			bool filterable,
			IReadOnlyCollection<object> filterOptions,
			Func<object, string>? filterOptionDisplayNameSelector,
			IReadOnlyDictionary<string, object> additionalAttributes,
			UmbrellaColumnFilterType filterControlType,
			FilterType filterMatchType,
			UmbrellaColumnFilterOptionsType? filterOptionsType,
			string? propertyName,
			string? filterMemberPathOverride,
			string? sorterMemberPathOverride,
			UmbrellaColumnDisplayMode displayMode)
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
			PropertyName = propertyName;
			FilterMemberPathOverride = filterMemberPathOverride;
			SorterMemberPathOverride = sorterMemberPathOverride;
			DisplayMode = displayMode;

			if (FilterOptions.Count > 0)
			{
				FilterControlType = UmbrellaColumnFilterType.Options;
				FilterMatchType = FilterType.Equal;
			}
			else if (FilterOptionsType == UmbrellaColumnFilterOptionsType.Boolean)
			{
				FilterControlType = UmbrellaColumnFilterType.Options;
				FilterMatchType = FilterType.Equal;
				FilterOptions = new[] { "Yes", "No" };
			}
		}

		/// <summary>
		/// Gets the property name.
		/// </summary>
		public string? PropertyName { get; }

		/// <summary>
		/// Gets or sets the property path override used as the <see cref="FilterExpressionDescriptor.MemberPath"/> property value when
		/// creating filters.
		/// </summary>
		/// <remarks>
		/// If this value is <see langword="null"/>, the <see cref="PropertyName"/> will be used.
		/// </remarks>
		public string? FilterMemberPathOverride { get; }

		/// <summary>
		/// Gets or sets the property path override used as the value <see cref="SortExpressionDescriptor.MemberPath"/> property value when
		/// creating sorters.
		/// </summary>
		/// <remarks>
		/// If this value is <see langword="null"/>, the <see cref="PropertyName"/> will be used.
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
		public IReadOnlyCollection<object> FilterOptions { get; }

		/// <summary>
		/// Specifies the type of the <see cref="FilterOptions"/> displayed to the user.
		/// </summary>
		public UmbrellaColumnFilterOptionsType? FilterOptionsType { get; }

		/// <summary>
		/// Gets a delegate used to convert a filter option to a friendly string for displaying to the user.
		/// </summary>
		public Func<object, string>? FilterOptionDisplayNameSelector { get; }

		/// <summary>
		/// Gets a collection of the unmatched parameter values specified on the <see cref="UmbrellaColumn"/> component that this instance is associated with.
		/// </summary>
		public IReadOnlyDictionary<string, object> AdditionalAttributes { get; }
		
		/// <summary>
		/// Gets or sets the value used to filter the column.
		/// </summary>
		public string? FilterValue { get; set; }

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

				return option.ToString();
			}

			return FilterOptionDisplayNameSelector(option);
		}
	}
}