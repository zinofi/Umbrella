// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.DynamicImage.Caching.Disk;

/// <summary>
/// Specifies caching options when storing generated images on disk.
/// </summary>
/// <seealso cref="IValidatableUmbrellaOptions" />
/// <seealso cref="ISanitizableUmbrellaOptions" />
public class DynamicImageDiskCacheOptions : IValidatableUmbrellaOptions, ISanitizableUmbrellaOptions
{
	/// <summary>
	/// Gets or sets the name of the cache folder. Defaults to "DynamicImageCache".
	/// </summary>
	public string CacheFolderName { get; set; } = "DynamicImageCache";

	/// <inheritdoc />
	public void Sanitize() => CacheFolderName = CacheFolderName.Trim();

	/// <inheritdoc />
	public void Validate() => Guard.IsNotNullOrWhiteSpace(CacheFolderName);
}