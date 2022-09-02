﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.DynamicImage.Abstractions;

/// <summary>
/// DynamicImage Constants
/// </summary>
public class DynamicImageConstants
{
	/// <summary>
	/// The default path prefix used when generating dynamic image paths. This is primarily of use in web applications in conjunction with
	/// Middleware that parses request URLs and with TagHelpers and HtmlHelpers that create image URLs.
	/// </summary>
	public const string DefaultPathPrefix = "dynamicimage";
}