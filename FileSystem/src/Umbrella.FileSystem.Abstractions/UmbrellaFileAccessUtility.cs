// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading;
using Umbrella.Utilities.Context.Abstractions;

namespace Umbrella.FileSystem.Abstractions;

public abstract class UmbrellaFileAccessUtility<TUserId, TUserRoleType, TDirectoryType, TGroupId> : IUmbrellaFileAccessUtility<TDirectoryType, TGroupId>
	where TUserRoleType : struct, Enum
	where TDirectoryType : struct, Enum
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

	public virtual async Task<bool> CanAccessAsync(IUmbrellaFileInfo fileInfo, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			// TempFiles should only ever be accessible by the uploader.
			if (FileInfoIsInDirectory(fileInfo, TempFilesDirectoryName))
			{
				TUserId createdById = await fileInfo.GetCreatedByIdAsync<TUserId>(cancellationToken);

				if (createdById != null && createdById.Equals(CurrentUserIdAccessor.CurrentUserId))
					return true;
			}

			return false;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { fileInfo.SubPath, CurrentUserIdAccessor.CurrentUserId }))
		{
			throw new UmbrellaFileSystemException("There has been a problem checking the file permissions.", exc);
		}
	}

	public virtual async Task ApplyPermissionsAsync(IUmbrellaFileInfo fileInfo, TGroupId groupId, CancellationToken cancellationToken = default, bool writeChanges = true)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			if (FileInfoIsInDirectory(fileInfo, TempFilesDirectoryName))
				await fileInfo.SetCreatedByIdAsync(CurrentUserIdAccessor.CurrentUserId, cancellationToken, false);

			if (writeChanges)
				await fileInfo.WriteMetadataChangesAsync(cancellationToken);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { fileInfo.SubPath, CurrentUserIdAccessor.CurrentUserId, writeChanges }))
		{
			throw new UmbrellaFileSystemException("There has been a problem applying the required file permissions.", exc);
		}
	}

	public string GetTempDirectoryName() => $"/{TempFilesDirectoryName}";
	public string GetTempFilePath(string fileName) => $"{GetTempDirectoryName()}/{fileName}";
	public string GetTempWebFilePath(string fileName) => $"/{WebFolderName}{GetTempFilePath(fileName)}".ToLowerInvariant();
	public bool IsTempFilePath(string fileName) => fileName.StartsWith(GetTempDirectoryName() + "/", StringComparison.OrdinalIgnoreCase);

	public string GetDirectoryName(TDirectoryType directoryType, TGroupId groupId) => $"/{GetDirectoryNameFromType(directoryType)}/{groupId}";
	public string GetFilePath(TDirectoryType directoryType, TGroupId groupId, string fileName) => $"{GetDirectoryName(directoryType, groupId)}/{fileName}";
	public string GetWebFilePath(TDirectoryType directoryType, TGroupId groupId, string fileName) => $"/{WebFolderName}{GetFilePath(directoryType, groupId, fileName)}".ToLowerInvariant();

	protected bool FileInfoIsInDirectory(IUmbrellaFileInfo fi, string directoryName) => fi.SubPath.TrimStart('/').StartsWith($"{directoryName}/", StringComparison.OrdinalIgnoreCase);
	protected abstract string GetDirectoryNameFromType(TDirectoryType directoryType);
}