// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.Utilities.TypeConverters.Abstractions;

namespace Umbrella.FileSystem.Disk;

/// <summary>
/// An implementation of <see cref="UmbrellaFileStorageProvider{TFileInfo, TOptions}"/> which uses the physical disk as the underlying storage mechanism.
/// </summary>
/// <seealso cref="T:Umbrella.FileSystem.Disk.UmbrellaDiskFileProvider{Umbrella.FileSystem.Disk.UmbrellaDiskFileProviderOptions}" />
public class UmbrellaDiskFileStorageProvider : UmbrellaDiskFileStorageProvider<UmbrellaDiskFileStorageProviderOptions>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaDiskFileStorageProvider"/> class.
	/// </summary>
	/// <param name="loggerFactory">The logger factory.</param>
	/// <param name="mimeTypeUtility">The MIME type utility.</param>
	/// <param name="genericTypeConverter">The generic type converter.</param>
	public UmbrellaDiskFileStorageProvider(
		ILoggerFactory loggerFactory,
		IMimeTypeUtility mimeTypeUtility,
		IGenericTypeConverter genericTypeConverter)
		: base(loggerFactory, mimeTypeUtility, genericTypeConverter)
	{
	}
}
