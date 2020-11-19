using System.IO;
using System.Threading;

namespace Umbrella.AspNetCore.Components.Components.FileUpload
{
	public readonly struct UmbrellaFileUploadRequestEventArgs
	{
		public UmbrellaFileUploadRequestEventArgs(Stream content, string fileName, string type, CancellationToken uploadCancellationToken)
		{
			Content = content;
			FileName = fileName;
			Type = type;
			UploadCancellationToken = uploadCancellationToken;
		}

		public Stream Content { get; }
		public string FileName { get; }
		public string Type { get; }
		public CancellationToken UploadCancellationToken { get; }
	}
}