// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using System.ComponentModel;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.WebUtilities.DynamicImage.Middleware.Options;

/// <summary>
/// Options for implementations of the DynamicImageMiddleware in the ASP.NET and ASP.NET Core projects.
/// </summary>
/// <seealso cref="IValidatableUmbrellaOptions" />
/// <seealso cref="ISanitizableUmbrellaOptions" />
public class DynamicImageMiddlewareOptions : IValidatableUmbrellaOptions, ISanitizableUmbrellaOptions
{
	private Dictionary<string, DynamicImageMiddlewareMapping> _flattenedMappings = null!;

	/// <summary>
	/// Gets or sets the mappings.
	/// </summary>
	public List<DynamicImageMiddlewareMapping> Mappings { get; set; } = null!;

	/// <summary>
	/// Gets or sets the dynamic image path prefix. Defaults to <see cref="DynamicImageConstants.DefaultPathPrefix"/>.
	/// </summary>
	public string DynamicImagePathPrefix { get; set; } = DynamicImageConstants.DefaultPathPrefix;

	/// <summary>
	/// Gets or sets a value indicating whether Jpg images should be returned in WebP format for supported browsers.
	/// Defaults to <see langword="true"/>.
	/// </summary>
	public bool EnableJpgPngWebPOverride { get; set; } = true;

	/// <summary>
	/// Gets the file provider for the specified <paramref name="searchPath"/>.
	/// </summary>
	/// <param name="searchPath">The search path.</param>
	/// <returns></returns>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public DynamicImageMiddlewareMapping GetMapping(string searchPath)
	{
		Guard.IsNotNullOrWhiteSpace(searchPath, nameof(searchPath));

		return _flattenedMappings.SingleOrDefault(x => searchPath.Trim().StartsWith(x.Key, StringComparison.OrdinalIgnoreCase)).Value;
	}

	/// <inheritdoc />
	public void Sanitize()
	{
		if (Mappings != null)
		{
			Mappings.ForEach(x => x.Sanitize());
			_flattenedMappings = Mappings.SelectMany(x => x.FileProviderMapping.AppRelativeFolderPaths.ToDictionary(y => y, y => x)).ToDictionary(x => x.Key, x => x.Value);
		}

		DynamicImagePathPrefix = DynamicImagePathPrefix.Trim();
	}

	/// <inheritdoc />
	public void Validate()
	{
		Guard.IsNotNullOrEmpty(Mappings, nameof(Mappings));
		Guard.IsNotNullOrWhiteSpace(DynamicImagePathPrefix, nameof(DynamicImagePathPrefix));
		Guard.IsNotNullOrEmpty(_flattenedMappings, nameof(_flattenedMappings));
	}
}