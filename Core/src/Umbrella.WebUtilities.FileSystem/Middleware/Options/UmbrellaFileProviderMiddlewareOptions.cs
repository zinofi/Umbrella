using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Options.Abstractions;
using Umbrella.WebUtilities.Middleware.Options;

namespace Umbrella.WebUtilities.FileSystem.Middleware.Options
{
	/// <summary>
	/// Options for implementations of the UmbrellaFileProviderMiddleware in the ASP.NET and ASP.NET Core projects.
	/// </summary>
	/// <seealso cref="ISanitizableUmbrellaOptions" />
	/// <seealso cref="IValidatableUmbrellaOptions" />
	public class UmbrellaFileProviderMiddlewareOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
	{
		private Dictionary<string, UmbrellaFileProviderMiddlewareMapping> _flattenedMappings;

		/// <summary>
		/// Gets or sets the mappings.
		/// </summary>
		public List<UmbrellaFileProviderMiddlewareMapping> Mappings { get; set; }

		/// <summary>
		/// Gets the file provider for the specified <paramref name="searchPath"/>.
		/// </summary>
		/// <param name="searchPath">The search path.</param>
		/// <returns></returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public UmbrellaFileProviderMiddlewareMapping GetMapping(string searchPath)
		{
			Guard.ArgumentNotNullOrWhiteSpace(searchPath, nameof(searchPath));

			return _flattenedMappings.SingleOrDefault(x => searchPath.Trim().StartsWith(x.Key, StringComparison.OrdinalIgnoreCase)).Value;
		}

		/// <summary>
		/// Sanitizes this instance.
		/// </summary>
		public void Sanitize()
		{
			if (Mappings != null)
			{
				Mappings.ForEach(x => x.Sanitize());
				_flattenedMappings = Mappings.SelectMany(x => x.FileProviderMapping.AppRelativeFolderPaths.ToDictionary(y => y, y => x)).ToDictionary(x => x.Key, x => x.Value);
			}
		}

		/// <summary>
		/// Validates this instance.
		/// </summary>
		public void Validate()
		{
			Guard.ArgumentNotNullOrEmpty(Mappings, nameof(Mappings));
			Guard.ArgumentNotNullOrEmpty(_flattenedMappings, nameof(_flattenedMappings));
		}
	}
}