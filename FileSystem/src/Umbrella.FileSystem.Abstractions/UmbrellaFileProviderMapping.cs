using System.Collections.Generic;
using System.Linq;
using Umbrella.Utilities;
using Umbrella.Utilities.Exceptions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.FileSystem.Abstractions
{
	public class UmbrellaFileProviderMapping
	{
		public UmbrellaFileProviderMapping(IUmbrellaFileProvider fileProvider, params string[] appRelativeFolderPaths)
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
				throw new UmbrellaException($"The sanitized {nameof(appRelativeFolderPaths)} collection is empty. Please ensure some valid paths have been provided. All paths must have a leading forward slash '/'.");
		}

		public IUmbrellaFileProvider FileProvider { get; }
		public IReadOnlyCollection<string> AppRelativeFolderPaths { get; }
	}
}
