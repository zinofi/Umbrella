using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.DynamicImage.Caching
{
	/// <summary>
	/// Specifies caching options when storing generated images on disk.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.Options.Abstractions.IValidatableUmbrellaOptions" />
	/// <seealso cref="Umbrella.Utilities.Options.Abstractions.ISanitizableUmbrellaOptions" />
	public class DynamicImageDiskCacheOptions : IValidatableUmbrellaOptions, ISanitizableUmbrellaOptions
	{
		/// <summary>
		/// Gets or sets the name of the cache folder. Defaults to "DynamicImageCache".
		/// </summary>
		public string CacheFolderName { get; set; } = "DynamicImageCache";

		/// <summary>
		/// Sanitizes this instance.
		/// </summary>
		public void Sanitize() => CacheFolderName = CacheFolderName.TrimNull();

		/// <summary>
		/// Validates this instance.
		/// </summary>
		public void Validate() => Guard.ArgumentNotNullOrWhiteSpace(CacheFolderName, nameof(CacheFolderName));
	}
}