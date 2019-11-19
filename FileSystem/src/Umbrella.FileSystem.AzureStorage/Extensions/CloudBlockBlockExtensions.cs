using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Umbrella.FileSystem.AzureStorage.Extensions
{
	internal static class CloudBlockBlockExtensions
	{
		public static Task UploadFromByteArrayAsync(this CloudBlockBlob blob, byte[] buffer, int index, int count, CancellationToken cancellationToken)
			=> blob.UploadFromByteArrayAsync(buffer, index, count, null, null, null, cancellationToken);

		public static Task UploadFromStreamAsync(this CloudBlockBlob blob, Stream stream, CancellationToken cancellationToken)
			=> blob.UploadFromStreamAsync(stream, null, null, null, cancellationToken);

		public static Task<string> StartCopyAsync(this CloudBlockBlob blob, CloudBlockBlob source, CancellationToken cancellationToken)
			=> blob.StartCopyAsync(source, null, null, null, null, cancellationToken);
	}
}