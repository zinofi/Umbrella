using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;

namespace Umbrella.WebUtilities.FileSystem.Middleware.Options
{
	public class UmbrellaFileProviderMiddlewareOptions
	{
		// TODO: Somewhere in here it would be useful to allow the cache headers to be varied on a per provider basis.
		// This same approach should be adopted by both the FrontEndCompressionMiddleware and DynamicImageMiddleware
		// to allow for maximum flexibility.
		// Room for creating common abstractions too.
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

		public List<UmbrellaFileProviderMapping> Mappings { get; set; }

		[EditorBrowsable(EditorBrowsableState.Never)]
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

			PathMapping mapping = _flattenedMappingList.Find(x => searchPath.Trim().StartsWith(x.Path, StringComparison.OrdinalIgnoreCase));

			return mapping?.FileProvider;
		}
	}
}