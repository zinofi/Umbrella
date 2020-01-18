using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.DynamicImage.Caching.AzureStorage
{
	public class DynamicImageAzureBlobStorageCacheOptions : IValidatableUmbrellaOptions, ISanitizableUmbrellaOptions
	{
		/// <summary>
		/// The name of the container in which blobs are stored. Defaults to "dynamicimagecache".
		/// </summary>
		public string ContainerName { get; set; } = "dynamicimagecache";

		public void Sanitize() => ContainerName = ContainerName?.TrimToLowerInvariant();

		public void Validate() => Guard.ArgumentNotNullOrWhiteSpace(ContainerName, nameof(ContainerName));
	}
}