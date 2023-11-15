﻿namespace Umbrella.FileSystem.Abstractions;

/// <summary>
/// Constants used with the Umbrella file system.
/// </summary>
public static class UmbrellaFileSystemConstants
{
	/// <summary>
	/// A constant representing a kilo-byte (Non-SI version).
	/// </summary>
	public const long KB = 1024;

	/// <summary>
	/// A constant representing a megabyte (Non-SI version).
	/// </summary>
	public const long MB = 1024 * KB;

	/// <summary>
	/// A constant representing a gigabyte (Non-SI version).
	/// </summary>
	public const long GB = 1024 * MB;

	/// <summary>
	/// Small buffer size better suited for use with the UmbrellaDiskFileProvider and UmbrellaDiskFileInfo.
	/// </summary>
	public const int SmallBufferSize = (int)(4 * KB);

	/// <summary>
	/// Large buffer size better suited for use with the UmbrellaAzureBlobStorageFileProvider and UmbrellaAzureBlobStorageFileInfo.
	/// </summary>
	public const int LargeBufferSize = (int)(4 * MB);

	/// <summary>
	/// The metadata key for the CreatedById value.
	/// </summary>
	public const string CreatedByIdMetadataKey = "CreatedById";

	/// <summary>
	/// The metadata key for the FileName value.
	/// </summary>
	public const string FileNameMetadataKey = "FileName";

	/// <summary>
	/// The metadata key for the FileUploadType value.
	/// </summary>
	public const string FileUploadTypeMetadataKey = "FileUploadType";

	/// <summary>
	/// The default temporary files directory name.
	/// </summary>
	public const string DefaultTempFilesDirectoryName = "temp-files";

	/// <summary>
	/// The default web files directory name.
	/// </summary>
	public const string DefaultWebFilesDirectoryName = "files";
}