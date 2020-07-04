using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.DynamicImage.Caching.AzureStorage
{
	/// <summary>
	/// Specifies caching options when storing generated images using Azure Blob Storage.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.Options.Abstractions.IValidatableUmbrellaOptions" />
	/// <seealso cref="Umbrella.Utilities.Options.Abstractions.ISanitizableUmbrellaOptions" />
	public class DynamicImageAzureBlobStorageCacheOptions : IValidatableUmbrellaOptions, ISanitizableUmbrellaOptions
	{
		/// <summary>
		/// The name of the container in which blobs are stored. Defaults to "dynamicimagecache".
		/// </summary>
		public string ContainerName { get; set; } = "dynamicimagecache";

		/// <inheritdoc />
		public void Sanitize() => ContainerName = ContainerName?.TrimToLowerInvariant();

		/// <inheritdoc />
		public void Validate() => Guard.ArgumentNotNullOrWhiteSpace(ContainerName, nameof(ContainerName));
	}
}