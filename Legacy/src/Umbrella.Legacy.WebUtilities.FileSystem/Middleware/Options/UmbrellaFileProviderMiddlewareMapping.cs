using System.Collections.Generic;
using System.Linq;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.WebUtilities.Exceptions;

namespace Umbrella.Legacy.WebUtilities.FileSystem.Middleware.Options
{
	// TODO: Move to a common project somewhere.
	public class UmbrellaFileProviderMiddlewareMapping
	{
		public UmbrellaFileProviderMiddlewareMapping(IUmbrellaFileProvider fileProvider, params string[] appRelativeFolderPaths)
		{
			Guard.ArgumentNotNull(fileProvider, nameof(fileProvider));
			Guard.ArgumentNotNullOrEmpty(appRelativeFolderPaths, nameof(appRelativeFolderPaths));

			FileProvider = fileProvider;

			// Sanitize the paths
			AppRelativeFolderPaths = appRelativeFolderPaths
				.Where(x => !string.IsNullOrWhiteSpace(x) && x[0] == '/')
				.Select(x => x.TrimToLowerInvariant())
				.Distinct()
				.ToList()
				.AsReadOnly();

			// Validate the end result of the paths cleanup.
			if (AppRelativeFolderPaths.Count == 0)
				throw new UmbrellaWebException($"The sanitized {nameof(appRelativeFolderPaths)} collection is empty. Please ensure some valid paths have been provided. All paths must have a leading forward slash '/'.");
		}

		public IUmbrellaFileProvider FileProvider { get; }
		public IReadOnlyCollection<string> AppRelativeFolderPaths { get; }
	}
}