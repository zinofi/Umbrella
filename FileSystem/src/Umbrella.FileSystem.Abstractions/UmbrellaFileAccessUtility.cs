// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading;
using Umbrella.Utilities.Context.Abstractions;

namespace Umbrella.FileSystem.Abstractions;

/// <summary>
/// A base class which provides utilities for checking access to files, applying file permissions
/// which can be checked before loading files, and also utility methods for generating file paths based on specified
/// file instances and directories.
/// </summary>
/// <typeparam name="TUserId">The type of the user identifier.</typeparam>
/// <typeparam name="TUserRoleType">The type of the user role type.</typeparam>
/// <typeparam name="TDirectoryType">The type of the directory type.</typeparam>
/// <typeparam name="TGroupId">The type of the group identifier.</typeparam>
/// <seealso cref="IUmbrellaFileAccessUtility{TDirectoryType, TGroupId}" />
public abstract class UmbrellaFileAccessUtility<TUserId, TUserRoleType, TDirectoryType, TGroupId> : IUmbrellaFileAccessUtility<TDirectoryType, TGroupId>
	where TUserRoleType : struct, Enum
	where TDirectoryType : struct, Enum
	where TGroupId : IEquatable<TGroupId>
{
	/// <summary>
	/// Gets the logger.
	/// </summary>
	protected ILogger Logger { get; }

	/// <summary>
	/// Gets the current user identifier accessor.
	/// </summary>
	protected ICurrentUserIdAccessor<TUserId> CurrentUserIdAccessor { get; }

	/// <summary>
	/// Gets the current user roles accessor.
	/// </summary>
	protected ICurrentUserRolesAccessor<TUserRoleType> CurrentUserRolesAccessor { get; }

	/// <summary>
	/// Gets the current user claims principal accessor.
	/// </summary>
	protected ICurrentUserClaimsPrincipalAccessor CurrentUserClaimsPrincipalAccessor { get; }

	/// <summary>
	/// Gets the name of the temporary files directory, e.g. temp-files
	/// </summary>
	protected abstract string TempFilesDirectoryName { get; }

	/// <summary>
	/// Gets the name of the web folder, e.g. files
	/// </summary>
	protected abstract string WebFolderName { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaFileAccessUtility{TUserId, TUserRoleType, TDirectoryType, TGroupId}"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="currentUserIdAccessor">The current user identifier accessor.</param>
	/// <param name="currentUserRolesAccessor">The current user roles accessor.</param>
	/// <param name="currentUserClaimsPrincipalAccessor">The current user claims principal accessor.</param>
	protected UmbrellaFileAccessUtility(
		ILogger logger,
		ICurrentUserIdAccessor<TUserId> currentUserIdAccessor,
		ICurrentUserRolesAccessor<TUserRoleType> currentUserRolesAccessor,
		ICurrentUserClaimsPrincipalAccessor currentUserClaimsPrincipalAccessor)
	{
		Logger = logger;
		CurrentUserIdAccessor = currentUserIdAccessor;
		CurrentUserRolesAccessor = currentUserRolesAccessor;
		CurrentUserClaimsPrincipalAccessor = currentUserClaimsPrincipalAccessor;
	}
	
	/// <inheritdoc/>
	public virtual async Task<bool> CanAccessAsync(IUmbrellaFileInfo fileInfo, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			// TempFiles should only ever be accessible by the uploader.
			if (FileInfoIsInDirectory(fileInfo, TempFilesDirectoryName))
			{
				TUserId createdById = await fileInfo.GetCreatedByIdAsync<TUserId>(cancellationToken);

				if (createdById is not null && createdById.Equals(CurrentUserIdAccessor.CurrentUserId))
					return true;
			}

			return false;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { fileInfo.SubPath, CurrentUserIdAccessor.CurrentUserId }))
		{
			throw new UmbrellaFileSystemException("There has been a problem checking the file permissions.", exc);
		}
	}

	/// <inheritdoc/>
	public virtual async Task ApplyPermissionsAsync(IUmbrellaFileInfo fileInfo, TGroupId groupId, bool writeChanges = true, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			if (FileInfoIsInDirectory(fileInfo, TempFilesDirectoryName))
				await fileInfo.SetCreatedByIdAsync(CurrentUserIdAccessor.CurrentUserId, cancellationToken, false);

			if (writeChanges)
				await fileInfo.WriteMetadataChangesAsync(cancellationToken);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { fileInfo.SubPath, CurrentUserIdAccessor.CurrentUserId, groupId, writeChanges }))
		{
			throw new UmbrellaFileSystemException("There has been a problem applying the required file permissions.", exc);
		}
	}

	/// <inheritdoc/>
	public string GetTempDirectoryName() => $"/{TempFilesDirectoryName}";

	/// <inheritdoc/>
	public string GetTempFilePath(string fileName) => $"{GetTempDirectoryName()}/{fileName}";

	/// <inheritdoc/>
	public string GetTempWebFilePath(string fileName) => $"/{WebFolderName}{GetTempFilePath(fileName)}".ToLowerInvariant();

	/// <inheritdoc/>
	public bool IsTempFilePath(string fileName) => fileName.StartsWith(GetTempDirectoryName() + "/", StringComparison.OrdinalIgnoreCase);

	/// <inheritdoc/>
	public string GetDirectoryName(TDirectoryType directoryType, TGroupId groupId) => $"/{GetDirectoryNameFromType(directoryType)}/{groupId}";

	/// <inheritdoc/>
	public string GetFilePath(TDirectoryType directoryType, string fileName, TGroupId groupId) => $"{GetDirectoryName(directoryType, groupId)}/{fileName}";

	/// <inheritdoc/>
	public string GetWebFilePath(TDirectoryType directoryType, string fileName, TGroupId groupId) => $"/{WebFolderName}{GetFilePath(directoryType, fileName, groupId)}".ToLowerInvariant();

	/// <summary>
	/// Determines if the specified file is inside the specified <paramref name="directoryName"/>.
	/// </summary>
	/// <param name="file">The file.</param>
	/// <param name="directoryName">Name of the directory.</param>
	/// <returns></returns>
	protected bool FileInfoIsInDirectory(IUmbrellaFileInfo file, string directoryName) => file.SubPath.TrimStart('/').StartsWith($"{directoryName}/", StringComparison.OrdinalIgnoreCase);

	/// <summary>
	/// Gets the directory name from the specified enum <paramref name="directoryType"/> parameter.
	/// </summary>
	/// <param name="directoryType">Type of the directory.</param>
	/// <returns>The directory name.</returns>
	/// <remarks>
	/// This should return a directory name for the specified enum, e.g. an enum value of
	/// "UserDocuments" should return a value, e.g. "user-documents".
	/// </remarks>
	protected abstract string GetDirectoryNameFromType(TDirectoryType directoryType);
}