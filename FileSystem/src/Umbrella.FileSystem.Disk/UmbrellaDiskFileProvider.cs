using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.Utilities.TypeConverters.Abstractions;

namespace Umbrella.FileSystem.Disk
{
	public class UmbrellaDiskFileProvider : UmbrellaDiskFileProvider<UmbrellaDiskFileProviderOptions>
	{
		public UmbrellaDiskFileProvider(
			ILoggerFactory loggerFactory,
			IMimeTypeUtility mimeTypeUtility,
			IGenericTypeConverter genericTypeConverter)
			: base(loggerFactory, mimeTypeUtility, genericTypeConverter)
		{
		}
	}

	public class UmbrellaDiskFileProvider<TOptions> : UmbrellaFileProvider<UmbrellaDiskFileInfo, UmbrellaDiskFileProviderOptions>, IUmbrellaDiskFileProvider
		where TOptions : UmbrellaDiskFileProviderOptions
	{
		#region Constructors
		public UmbrellaDiskFileProvider(
			ILoggerFactory loggerFactory,
			IMimeTypeUtility mimeTypeUtility,
			IGenericTypeConverter genericTypeConverter)
			: base(loggerFactory.CreateLogger<UmbrellaDiskFileProvider>(), loggerFactory, mimeTypeUtility, genericTypeConverter)
		{
		}
		#endregion

		#region Overridden Methods
		protected override async Task<IUmbrellaFileInfo> GetFileAsync(string subpath, bool isNew, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

			// Sanitize subpath
			string coreSubpath = SanitizeSubPathCore(subpath);

			var cleanedPathBuilder = new StringBuilder(coreSubpath)
				.Replace('/', '\\')
				.Insert(0, Options.RootPhysicalPath + @"\");

			string physicalPath = cleanedPathBuilder.ToString();

			if (Log.IsEnabled(LogLevel.Debug))
				Log.WriteDebug(new { subpath, physicalPath });

			var physicalFileInfo = new FileInfo(physicalPath);

			if (!isNew && !physicalFileInfo.Exists)
				return null;

			var fileInfo = new UmbrellaDiskFileInfo(FileInfoLoggerInstance, MimeTypeUtility, GenericTypeConverter, subpath, this, physicalFileInfo, isNew);

			if (!await CheckFileAccessAsync(fileInfo, physicalFileInfo, cancellationToken))
				throw new UmbrellaFileAccessDeniedException(subpath);

			return fileInfo;
		}
		#endregion

		#region Protected Methods
		protected virtual Task<bool> CheckFileAccessAsync(UmbrellaDiskFileInfo fileInfo, FileInfo physicalFileInfo, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			return Task.FromResult(true);
		}
		#endregion
	}
}