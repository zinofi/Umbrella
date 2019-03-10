using System;
using System.Collections.Generic;
using System.Linq;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.WebUtilities.Exceptions;
using Umbrella.Utilities.Comparers;

namespace Umbrella.Legacy.WebUtilities.FileSystem.Middleware.Options
{
	public class UmbrellaFileProviderMiddlewareOptions
	{
		private class PathMapping
		{
			public PathMapping(string path, IUmbrellaFileProvider fileProvider)
			{
				Path = path;
				FileProvider = fileProvider;
			}

			public string Path { get; }
			public IUmbrellaFileProvider FileProvider { get; }
		}

		private readonly object _syncRoot = new object();
		private List<PathMapping> _flattenedMappingList;

		public List<UmbrellaFileProviderMiddlewareMapping> Mappings { get; set; }

		public IUmbrellaFileProvider GetFileProvider(string searchPath)
		{
			Guard.ArgumentNotNullOrWhiteSpace(searchPath, nameof(searchPath));

			if (_flattenedMappingList == null)
			{
				lock (_syncRoot)
				{
					if (_flattenedMappingList == null)
					{
						// Ensure that across all mappings the paths are unique. Not going overboard here as ultimately it's up to
						// the consumer of this middleware to not do daft stuff.
						_flattenedMappingList = Mappings.SelectMany(x => x.AppRelativeFolderPaths.Select(y => new PathMapping(y, x.FileProvider))).ToList();
					}
				}
			}

			var mapping = _flattenedMappingList.Find(x => searchPath.Trim().StartsWith(x.Path, StringComparison.OrdinalIgnoreCase));

			return mapping?.FileProvider;
		}
	}

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