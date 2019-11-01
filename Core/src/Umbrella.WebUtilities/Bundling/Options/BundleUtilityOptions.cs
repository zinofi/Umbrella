using Microsoft.Extensions.Caching.Memory;
using Umbrella.Utilities;
using Umbrella.Utilities.Options;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.WebUtilities.Bundling.Options
{
	public class BundleUtilityOptions : CacheableUmbrellaOptions, ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
	{
		public string DefaultBundleFolderAppRelativePath { get; set; }
		public bool WatchFiles { get; set; }
		public bool? AppendVersion { get; set; }
		public override CacheItemPriority CachePriority { get; set; } = CacheItemPriority.NeverRemove;

		/// <summary>
		/// Sanitizes this instance.
		/// </summary>
		public virtual void Sanitize()
		{
			// Ensure the path ends with a slash
			if (!DefaultBundleFolderAppRelativePath?.EndsWith("/") == true)
				DefaultBundleFolderAppRelativePath += "/";
		}

		/// <summary>
		/// Validates this instance.
		/// </summary>
		public virtual void Validate() => Guard.ArgumentNotNullOrWhiteSpace(DefaultBundleFolderAppRelativePath, nameof(DefaultBundleFolderAppRelativePath));
	}
}