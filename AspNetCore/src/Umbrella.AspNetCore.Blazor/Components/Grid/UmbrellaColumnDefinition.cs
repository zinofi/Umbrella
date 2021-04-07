using System;
using System.Collections.Generic;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.AspNetCore.Blazor.Components.Grid
{
	public class UmbrellaColumnDefinition
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaColumnDefinition"/> class.
		/// </summary>
		/// <param name="heading">The heading.</param>
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
		public UmbrellaColumnDefinition(string? heading, int? percentageWidth, bool sortable, bool filterable, IReadOnlyCollection<object> filterOptions, Func<object, string>? filterOptionDisplayNameSelector, IReadOnlyDictionary<string, object> additionalAttributes, UmbrellaColumnFilterType filterControlType, FilterType filterMatchType, UmbrellaColumnFilterOptionsType? filterOptionsType, string? propertyName)
		{
			Heading = heading;
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

			if (FilterOptions.Count > 0)
			{
				FilterControlType = UmbrellaColumnFilterType.Options;
				FilterMatchType = FilterType.Equal;
			}
			else if(FilterOptionsType == UmbrellaColumnFilterOptionsType.Boolean)
			{
				FilterControlType = UmbrellaColumnFilterType.Options;
				FilterMatchType = FilterType.Equal;
				FilterOptions = new[] { "Yes", "No" };
			}
		}

		public string? PropertyName { get; }
		public string? Heading { get; }
		public int? PercentageWidth { get; set; }
		public bool Sortable { get; }
		public bool Filterable { get; }
		public UmbrellaColumnFilterType FilterControlType { get; }
		public FilterType FilterMatchType { get; }
		public IReadOnlyCollection<object> FilterOptions { get; }
		public UmbrellaColumnFilterOptionsType? FilterOptionsType { get; }
		public Func<object, string>? FilterOptionDisplayNameSelector { get; set; }
		public IReadOnlyDictionary<string, object> AdditionalAttributes { get; }
		public string? FilterValue { get; set; }
		public SortDirection? Direction { get; set; }

		public string DirectionCssClass => Direction switch
		{
			SortDirection.Ascending => "fas fa-sort-up",
			SortDirection.Descending => "fas fa-sort-down",
			_ => "fas fa-sort",
		};

		public string ColumnWidthCssClass => PercentageWidth.HasValue ? $"u-grid__column--{PercentageWidth}" : "u-grid__column--auto";

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