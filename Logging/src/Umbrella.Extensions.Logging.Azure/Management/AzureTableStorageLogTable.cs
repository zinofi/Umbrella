// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.Extensions.Logging.Azure.Management;

/// <summary>
/// Represents a log table stored in Azure Table Storage.
/// </summary>
public class AzureTableStorageLogTable
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AzureTableStorageLogTable"/> class.
	/// </summary>
	/// <param name="date">The date.</param>
	/// <param name="name">The name.</param>
	public AzureTableStorageLogTable(DateTime date, string name)
	{
		Date = date;
		Name = name;
	}

	/// <summary>
	/// Gets the date.
	/// </summary>
	public DateTime Date { get; }

	/// <summary>
	/// Gets the name.
	/// </summary>
	public string Name { get; }
}