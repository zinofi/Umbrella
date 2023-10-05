// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.DynamicImage.Mvc.TagHelpers.Options;

/// <summary>
/// Options for use with Dynamic Image Tag Helpers.
/// </summary>
/// <seealso cref="ISanitizableUmbrellaOptions" />
/// <seealso cref="IValidatableUmbrellaOptions" />
public class DynamicImageTagHelperOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
{
	/// <summary>
	/// Gets or sets the dynamic image path prefix. Defaults to <see cref="DynamicImageConstants.DefaultPathPrefix"/>.
	/// </summary>
	public string DynamicImagePathPrefix { get; set; } = DynamicImageConstants.DefaultPathPrefix;

	/// <inheritdoc />
	public void Sanitize() => DynamicImagePathPrefix = DynamicImagePathPrefix.Trim();

	/// <inheritdoc />
	public void Validate() => Guard.IsNotNullOrWhiteSpace(DynamicImagePathPrefix);
}