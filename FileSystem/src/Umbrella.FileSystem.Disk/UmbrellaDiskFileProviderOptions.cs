using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Helpers;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.FileSystem.Disk
{
	/// <summary>
	/// Options for the <see cref="UmbrellaDiskFileProvider"/>.
	/// </summary>
	/// <seealso cref="IUmbrellaFileProviderOptions" />
	/// <seealso cref="ISanitizableUmbrellaOptions" />
	/// <seealso cref="IValidatableUmbrellaOptions" />
	public class UmbrellaDiskFileProviderOptions : IUmbrellaFileProviderOptions, ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
	{
		/// <summary>
		/// Gets or sets the physical root path on disk for the file provider.
		/// </summary>
		public string RootPhysicalPath { get; set; }

		/// <summary>
		/// Sanitizes this instance.
		/// </summary>
		public void Sanitize() => RootPhysicalPath = PathHelper.PlatformNormalize(RootPhysicalPath?.Trim()?.TrimEnd('\\'));

		/// <summary>
		/// Validates this instance.
		/// </summary>
		public void Validate() => Guard.ArgumentNotNullOrWhiteSpace(RootPhysicalPath, nameof(RootPhysicalPath));
	}
}