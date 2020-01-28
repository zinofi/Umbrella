using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

		public Task DeleteDirectoryAsync(string subpath, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

			try
			{
				string physicalPath = CleanPath(subpath);

				if (Log.IsEnabled(LogLevel.Debug))
					Log.WriteDebug(new { subpath, physicalPath }, "Directory");

				if (Directory.Exists(physicalPath))
				{
					try
					{
						Directory.Delete(physicalPath, true);
					}
					catch
					{
						Log.WriteWarning(state: new { subpath, physicalPath }, message: "The specified directory to be deleted no longer exists, most likely because of a race condition.");
					}
				}

				return Task.CompletedTask;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem deleting the specified directory.", exc);
			}
		}

		public async Task<IReadOnlyCollection<IUmbrellaFileInfo>> EnumerateDirectoryAsync(string subpath, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

			try
			{
				string physicalPath = CleanPath(subpath);

				if (Log.IsEnabled(LogLevel.Debug))
					Log.WriteDebug(new { subpath, physicalPath }, "Directory");

				var directoryInfo = new DirectoryInfo(physicalPath);

				if (!directoryInfo.Exists)
					return Array.Empty<UmbrellaDiskFileInfo>();

				UmbrellaDiskFileInfo[] files = directoryInfo
					.GetFiles()
					.Where(x => !x.Extension.Equals(UmbrellaDiskFileConstants.MetadataFileExtension, StringComparison.OrdinalIgnoreCase))
					.Select(x => new UmbrellaDiskFileInfo(FileInfoLoggerInstance, MimeTypeUtility, GenericTypeConverter, $"{subpath}/{x.Name}", this, x, false))
					.ToArray();

				var lstResult = new List<UmbrellaDiskFileInfo>();

				foreach(var file in files)
				{
					if (await CheckFileAccessAsync(file, file.PhysicalFileInfo, cancellationToken))
						lstResult.Add(file);
					else
						Log.WriteWarning(state: new { file.SubPath }, message: "File access failed.");
				}

				return lstResult;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { subpath }, returnValue: true))
			{
				throw new UmbrellaFileSystemException("There has been a problem enumerating the files in the specified directory.", exc);
			}
		}

		#region Overridden Methods
		protected override async Task<IUmbrellaFileInfo> GetFileAsync(string subpath, bool isNew, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			Guard.ArgumentNotNullOrWhiteSpace(subpath, nameof(subpath));

			string physicalPath = CleanPath(subpath);

			if (Log.IsEnabled(LogLevel.Debug))
				Log.WriteDebug(new { subpath, physicalPath }, "File");

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

		#region Private Methods
		private string CleanPath(string subpath)
		{
			// Sanitize subpath
			string coreSubpath = SanitizeSubPathCore(subpath);

			var cleanedPathBuilder = new StringBuilder(coreSubpath)
				.Replace('/', '\\')
				.Insert(0, Options.RootPhysicalPath + @"\");

			return cleanedPathBuilder.ToString();
		}
		#endregion
	}
}