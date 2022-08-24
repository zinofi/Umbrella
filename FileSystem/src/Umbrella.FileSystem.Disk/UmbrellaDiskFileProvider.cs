﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Text;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities.Helpers;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.Utilities.TypeConverters.Abstractions;

namespace Umbrella.FileSystem.Disk;

/// <summary>
/// An implementation of <see cref="UmbrellaFileProvider{TFileInfo, TOptions}"/> which uses the physical disk as the underlying storage mechanism.
/// </summary>
/// <seealso cref="T:Umbrella.FileSystem.Disk.UmbrellaDiskFileProvider{Umbrella.FileSystem.Disk.UmbrellaDiskFileProviderOptions}" />
public class UmbrellaDiskFileProvider : UmbrellaDiskFileProvider<UmbrellaDiskFileProviderOptions>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDiskFileProvider"/> class.
	/// </summary>
	/// <param name="loggerFactory">The logger factory.</param>
	/// <param name="mimeTypeUtility">The MIME type utility.</param>
	/// <param name="genericTypeConverter">The generic type converter.</param>
	public UmbrellaDiskFileProvider(
		ILoggerFactory loggerFactory,
		IMimeTypeUtility mimeTypeUtility,
		IGenericTypeConverter genericTypeConverter)
		: base(loggerFactory, mimeTypeUtility, genericTypeConverter)
	{
	}
}

/// <summary>
/// An implementation of <see cref="UmbrellaFileProvider{TFileInfo, TOptions}"/> which uses the physical disk as the underlying storage mechanism.
/// </summary>
/// <typeparam name="TOptions">The type of the options.</typeparam>
/// <seealso cref="T:Umbrella.FileSystem.Disk.UmbrellaDiskFileProvider{Umbrella.FileSystem.Disk.UmbrellaDiskFileProviderOptions}" />
public class UmbrellaDiskFileProvider<TOptions> : UmbrellaFileProvider<UmbrellaDiskFileInfo, UmbrellaDiskFileProviderOptions>, IUmbrellaDiskFileProvider
	where TOptions : UmbrellaDiskFileProviderOptions
{
	#region Constructors		
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDiskFileProvider{TOptions}"/> class.
	/// </summary>
	/// <param name="loggerFactory">The logger factory.</param>
	/// <param name="mimeTypeUtility">The MIME type utility.</param>
	/// <param name="genericTypeConverter">The generic type converter.</param>
	public UmbrellaDiskFileProvider(
		ILoggerFactory loggerFactory,
		IMimeTypeUtility mimeTypeUtility,
		IGenericTypeConverter genericTypeConverter)
		: base(loggerFactory.CreateLogger<UmbrellaDiskFileProvider>(), loggerFactory, mimeTypeUtility, genericTypeConverter)
	{
	}
	#endregion

	/// <inheritdoc />
	public Task DeleteDirectoryAsync(string subpath, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(subpath, nameof(subpath));

		try
		{
			string physicalPath = CleanPath(subpath);

			if (Logger.IsEnabled(LogLevel.Debug))
				Logger.WriteDebug(new { subpath, physicalPath }, "Directory");

			if (Directory.Exists(physicalPath))
			{
				try
				{
					Directory.Delete(physicalPath, true);
				}
				catch
				{
					_ = Logger.WriteWarning(state: new { subpath, physicalPath }, message: "The specified directory to be deleted no longer exists, most likely because of a race condition.");
				}
			}

			return Task.CompletedTask;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { subpath }, returnValue: true))
		{
			throw new UmbrellaFileSystemException("There has been a problem deleting the specified directory.", exc);
		}
	}

	/// <inheritdoc />
	public async Task<IReadOnlyCollection<IUmbrellaFileInfo>> EnumerateDirectoryAsync(string subpath, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(subpath, nameof(subpath));

		try
		{
			string physicalPath = CleanPath(subpath);

			if (Logger.IsEnabled(LogLevel.Debug))
				Logger.WriteDebug(new { subpath, physicalPath }, "Directory");

			var directoryInfo = new DirectoryInfo(physicalPath);

			if (!directoryInfo.Exists)
				return Array.Empty<UmbrellaDiskFileInfo>();

			UmbrellaDiskFileInfo[] files = directoryInfo
				.GetFiles()
				.Where(x => !x.Extension.Equals(UmbrellaDiskFileConstants.MetadataFileExtension, StringComparison.OrdinalIgnoreCase))
				.Select(x => new UmbrellaDiskFileInfo(FileInfoLoggerInstance, MimeTypeUtility, GenericTypeConverter, $"{subpath}/{x.Name}", this, x, false))
				.ToArray();

			var lstResult = new List<UmbrellaDiskFileInfo>();

			foreach (var file in files)
			{
				if (await CheckFileAccessAsync(file, file.PhysicalFileInfo, cancellationToken))
					lstResult.Add(file);
				else
					_ = Logger.WriteWarning(state: new { file.SubPath }, message: "File access failed.");
			}

			return lstResult;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { subpath }, returnValue: true))
		{
			throw new UmbrellaFileSystemException("There has been a problem enumerating the files in the specified directory.", exc);
		}
	}

	#region Overridden Methods
	/// <inheritdoc />
	protected override async Task<IUmbrellaFileInfo?> GetFileAsync(string subpath, bool isNew, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		Guard.IsNotNullOrWhiteSpace(subpath, nameof(subpath));

		string physicalPath = CleanPath(subpath);
		string cleanedSubPath = PathHelper.PlatformNormalize(subpath);

		if (Logger.IsEnabled(LogLevel.Debug))
			Logger.WriteDebug(new { subpath, cleanedSubPath, physicalPath }, "File");

		var physicalFileInfo = new FileInfo(physicalPath);

		if (!isNew && !physicalFileInfo.Exists)
			return null;

		var fileInfo = new UmbrellaDiskFileInfo(FileInfoLoggerInstance, MimeTypeUtility, GenericTypeConverter, cleanedSubPath, this, physicalFileInfo, isNew);

		return !await CheckFileAccessAsync(fileInfo, physicalFileInfo, cancellationToken)
			? throw new UmbrellaFileAccessDeniedException(subpath)
			: (IUmbrellaFileInfo)fileInfo;
	}
	#endregion

	#region Protected Methods		
	/// <summary>
	/// Performs an access check on the file to ensure it can be accessed in the current context.
	/// </summary>
	/// <param name="fileInfo">The file information.</param>
	/// <param name="physicalFileInfo">The physical file information.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>An awaitable <see cref="Task"/> that returns <see langword="true" /> if the file passes the check; otherwise <see langword="false" />.</returns>
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

		var cleanedPathBuilder = new StringBuilder(coreSubpath);

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			_ = cleanedPathBuilder
				.Replace('/', '\\')
				.Insert(0, Options.RootPhysicalPath + @"\");
		}
		else
		{
			_ = cleanedPathBuilder.Insert(0, Options.RootPhysicalPath + "/");
		}

		return cleanedPathBuilder.ToString();
	}
	#endregion
}