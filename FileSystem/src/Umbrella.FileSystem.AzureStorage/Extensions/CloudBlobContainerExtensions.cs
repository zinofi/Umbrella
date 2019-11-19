using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Umbrella.FileSystem.AzureStorage.Extensions
{
	internal static class CloudBlobContainerExtensions
	{
		public static Task<bool> CreateIfNotExistsAsync(this CloudBlobContainer container, CancellationToken cancellationToken)
			=> container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Off, null, null, cancellationToken);
	}
}