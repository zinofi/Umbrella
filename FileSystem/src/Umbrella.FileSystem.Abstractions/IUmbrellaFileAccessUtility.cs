// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Security.Claims;

namespace Umbrella.FileSystem.Abstractions;

/// <summary>
/// An interface which provides utilities for checking access to files, applying file permissions
/// which can be checked before loading files, and also utility methods for generating file paths based on specified
/// file instances and directories.
/// </summary>
/// <typeparam name="TDirectoryType">The type of the directory type.</typeparam>
/// <typeparam name="TGroupId">The type of the group identifier.</typeparam>
public interface IUmbrellaFileAccessUtility<TDirectoryType, TGroupId>
	where TDirectoryType : struct, Enum
	where TGroupId : IEquatable<TGroupId>
{
	/// <summary>
	/// Applies permissions to the specified <paramref name="fileInfo"/> in the form of metadata.
	/// </summary>
	/// <param name="fileInfo">The file.</param>
	/// <param name="groupId">The optional group identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <param name="writeChanges">if set to <see langword="true" /> writes changes to the file metadata.</param>
	/// <returns>A task that completes when the operation has completed.</returns>
	/// <remarks>
	/// File permissions are applied as file metadata. Checks to determine if the file can be accessed in a specified context
	/// e.g. by the current <see cref="ClaimsPrincipal"/> are <c>not</c> done automatically.
	/// It is the responsibility of consuming code to call the <see cref="CanAccessAsync(IUmbrellaFileInfo, CancellationToken)"/> before allowing
	/// access to a file.
	/// </remarks>
	Task ApplyPermissionsAsync(IUmbrellaFileInfo fileInfo, TGroupId groupId, bool writeChanges = true, CancellationToken cancellationToken = default);

	/// <summary>
	/// Determines whether the specified <paramref name="fileInfo"/> can be accessed in the current context, e.g. by the current <see cref="ClaimsPrincipal"/>.
	/// </summary>
	/// <param name="fileInfo">The file.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns><see langword="true"/> if the file can be accessed; otherwise <see langword="false"/></returns>
	Task<bool> CanAccessAsync(IUmbrellaFileInfo fileInfo, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the name of the temporary directory.
	/// </summary>
	/// <returns>The directory name, e.g. /temp-files</returns>
	string GetTempDirectoryName();

	/// <summary>
	/// Gets the path of the specified <paramref name="fileName"/> combined with the temporary files directory.
	/// </summary>
	/// <param name="fileName">Name of the file.</param>
	/// <returns>The file path, e.g. /temp-files/file.png</returns>
	string GetTempFilePath(string fileName);

	/// <summary>
	/// Gets the path of the specified <paramref name="fileName"/> stored in the temporary files directory to be accessed via a URL.
	/// </summary>
	/// <param name="fileName">Name of the file.</param>
	/// <returns>The file path, e.g. /files/temp-files/file.png</returns>
	string GetTempWebFilePath(string fileName);

	/// <summary>
	/// Determines whether the specified <paramref name="filePath"/> is stored in the temporary files directory.
	/// </summary>
	/// <param name="filePath">Path of the file.</param>
	/// <returns><see langword="true"/> if the file can be accessed; otherwise <see langword="false"/></returns>
	bool IsTempFilePath(string filePath);

	/// <summary>
	/// Gets the name of the directory based on the specified <paramref name="directoryType"/> including the optional <paramref name="groupId"/>.
	/// </summary>
	/// <param name="directoryType">Type of the directory.</param>
	/// <param name="groupId">The optional group identifier.</param>
	/// <returns>The directory name, e.g. /documents or /documents/10 where the <paramref name="groupId"/> is 10.</returns>
	string GetDirectoryName(TDirectoryType directoryType, TGroupId groupId);

	/// <summary>
	/// Gets the file path based on the specified <paramref name="directoryType"/>, <paramref name="fileName"/> and optional <paramref name="groupId"/>.
	/// </summary>
	/// <param name="directoryType">Type of the directory.</param>
	/// <param name="fileName">Name of the file.</param>
	/// <param name="groupId">The optional group identifier.</param>
	/// <returns>The file path, e.g. /documents/file.png or /documents/10/file.png where the <paramref name="groupId"/> is 10.</returns>
	string GetFilePath(TDirectoryType directoryType, string fileName, TGroupId groupId);

	/// <summary>
	/// Gets the file path as it can be accessed using a URL based on the specified <paramref name="directoryType"/>, <paramref name="fileName"/> and optional <paramref name="groupId"/>.
	/// </summary>
	/// <param name="directoryType">Type of the directory.</param>
	/// <param name="fileName">Name of the file.</param>
	/// <param name="groupId">The optional group identifier.</param>
	/// <returns>The file path, e.g. /files/documents/file.png or /files/documents/10/file.png where the <paramref name="groupId"/> is 10.</returns>
	string GetWebFilePath(TDirectoryType directoryType, string fileName, TGroupId groupId);
}