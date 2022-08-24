// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Umbrella.Extensions.Logging.Azure.Management.Configuration;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.Extensions.Logging.Azure.Management.Options;

/// <summary>
/// The options for the <see cref="AzureTableStorageLogManager"/>.
/// </summary>
/// <seealso cref="IValidatableUmbrellaOptions" />
public class AzureTableStorageLogManagementOptions : IValidatableUmbrellaOptions
{
	/// <summary>
	/// Gets or sets the cache lifetime minutes.
	/// </summary>
	public int CacheLifetimeMinutes { get; set; } = 30;

	/// <summary>
	/// Gets or sets the azure storage connection string.
	/// </summary>
	public string AzureStorageConnectionString { get; set; } = null!;

	/// <summary>
	/// Gets the data sources.
	/// </summary>
	public List<AzureTableStorageLogDataSource> DataSources { get; } = new List<AzureTableStorageLogDataSource>();

	/// <inheritdoc />
	public void Validate()
	{
		Guard.IsNotNullOrWhiteSpace(AzureStorageConnectionString, nameof(AzureStorageConnectionString));
		Guard.IsGreaterThanOrEqualTo(CacheLifetimeMinutes, 0);
	}
}