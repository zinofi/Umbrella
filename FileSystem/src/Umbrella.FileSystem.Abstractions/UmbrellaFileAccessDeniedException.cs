// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.FileSystem.Abstractions;

/// <summary>
/// An exception thrown when there is a permissions based error that occurs when accessing a file.
/// </summary>
/// <seealso cref="UmbrellaFileSystemException" />
public class UmbrellaFileAccessDeniedException : UmbrellaFileSystemException
{
	/// <summary>
	/// Gets the subpath.
	/// </summary>
	public string Subpath { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaFileAccessDeniedException"/> class.
	/// </summary>
	/// <param name="subpath">The subpath.</param>
	public UmbrellaFileAccessDeniedException(string subpath)
		: base($"Access to the file located at {subpath} has been denied.")
	{
		Subpath = subpath;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaFileAccessDeniedException"/> class.
	/// </summary>
	/// <param name="subpath">The subpath.</param>
	/// <param name="innerException">The inner exception.</param>
	public UmbrellaFileAccessDeniedException(string subpath, Exception innerException)
		: base($"Access to the file located at {subpath} has been denied.", innerException)
	{
		Subpath = subpath;
	}
}