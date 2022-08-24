// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.DynamicImage.Caching.AzureStorage;

/// <summary>
/// Specifies caching options when storing generated images using Azure Blob Storage.
/// </summary>
/// <seealso cref="IValidatableUmbrellaOptions" />
/// <seealso cref="ISanitizableUmbrellaOptions" />
public class DynamicImageAzureBlobStorageCacheOptions : IValidatableUmbrellaOptions, ISanitizableUmbrellaOptions
{
	/// <summary>
	/// The name of the container in which blobs are stored. Defaults to "dynamicimagecache".
	/// </summary>
	public string ContainerName { get; set; } = "dynamicimagecache";

	/// <inheritdoc />
	public void Sanitize() => ContainerName = ContainerName.TrimToLowerInvariant();

	/// <inheritdoc />
	public void Validate() => Guard.IsNotNullOrWhiteSpace(ContainerName, nameof(ContainerName));
}