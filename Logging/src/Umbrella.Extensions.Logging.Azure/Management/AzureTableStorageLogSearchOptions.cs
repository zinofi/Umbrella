// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.Extensions.Logging.Azure.Management;

/// <summary>
/// Options for search logs stored in Azure Table Storage.
/// </summary>
public class AzureTableStorageLogSearchOptions
{
	/// <summary>
	/// Gets or sets the property to sort on.
	/// </summary>
	public string? SortProperty { get; set; }

	/// <summary>
	/// Gets or sets the sort direction.
	/// </summary>
	public SortDirection SortDirection { get; set; }

	/// <summary>
	/// Gets or sets the page number.
	/// </summary>
	public int PageNumber { get; set; }

	/// <summary>
	/// Gets or sets the size of the page.
	/// </summary>
	public int PageSize { get; set; }

	/// <summary>
	/// Gets or sets the filters.
	/// </summary>
	public Dictionary<string, string>? Filters { get; set; }
}