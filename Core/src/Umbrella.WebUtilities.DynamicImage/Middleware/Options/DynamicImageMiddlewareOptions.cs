using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.WebUtilities.DynamicImage.Middleware.Options
{
	/// <summary>
	/// Options for implementations of the DynamicImageMiddleware in the ASP.NET and ASP.NET Core projects.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.Options.Abstractions.IValidatableUmbrellaOptions" />
	/// <seealso cref="Umbrella.Utilities.Options.Abstractions.ISanitizableUmbrellaOptions" />
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
			Guard.ArgumentNotNullOrWhiteSpace(searchPath, nameof(searchPath));

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
			Guard.ArgumentNotNullOrEmpty(Mappings, nameof(Mappings));
			Guard.ArgumentNotNullOrWhiteSpace(DynamicImagePathPrefix, nameof(DynamicImagePathPrefix));
			Guard.ArgumentNotNullOrEmpty(_flattenedMappings, nameof(_flattenedMappings));
		}
	}
}