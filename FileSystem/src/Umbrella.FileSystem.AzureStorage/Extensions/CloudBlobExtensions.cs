using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Umbrella.FileSystem.AzureStorage.Extensions
{
	internal static class CloudBlobExtensions
	{
		public static Task<bool> ExistsAsync(this CloudBlob blob, CancellationToken cancellationToken)
			=> blob.ExistsAsync(null, null, cancellationToken);

		public static Task<bool> DeleteIfExistsAsync(this CloudBlob blob, CancellationToken cancellationToken)
			=> blob.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, null, null, cancellationToken);

		public static Task<int> DownloadToByteArrayAsync(this CloudBlob blob, byte[] target, int index, CancellationToken cancellationToken)
			=> blob.DownloadToByteArrayAsync(target, index, null, null, null, cancellationToken);

		public static Task DownloadToStreamAsync(this CloudBlob blob, Stream target, CancellationToken cancellationToken)
			=> blob.DownloadToStreamAsync(target, null, null, null, cancellationToken);

		public static Task<Stream> OpenReadAsync(this CloudBlob blob, CancellationToken cancellationToken)
			=> blob.OpenReadAsync(null, null, null, cancellationToken);

		public static Task SetMetadataAsync(this CloudBlob blob, CancellationToken cancellationToken)
			=> blob.SetMetadataAsync(null, null, null, cancellationToken);
	}
}