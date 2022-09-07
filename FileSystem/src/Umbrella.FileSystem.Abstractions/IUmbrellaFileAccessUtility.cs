// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.FileSystem.Abstractions;

/// <summary>
/// A base class which provides utilities for checking access to files, applying file permissions
/// which can be checked before loading files, and also utility methods for generating file paths based on specified
/// file instances and directories.
/// </summary>
/// <typeparam name="TDirectoryType">The type of the directory type.</typeparam>
/// <typeparam name="TGroupId">The type of the group identifier.</typeparam>
public interface IUmbrellaFileAccessUtility<TDirectoryType, TGroupId>
	where TDirectoryType : struct, Enum
{
	Task ApplyPermissionsAsync(IUmbrellaFileInfo fileInfo, TGroupId? groupId, CancellationToken cancellationToken = default, bool writeChanges = true);
	Task<bool> CanAccessAsync(IUmbrellaFileInfo fileInfo, CancellationToken cancellationToken = default);
	string GetTempDirectoryName();
	string GetTempFilePath(string fileName);
	string GetTempWebFilePath(string fileName);
	bool IsTempFilePath(string fileName);
	string GetDirectoryName(TDirectoryType directoryType, TGroupId? groupId);
	string GetFilePath(TDirectoryType directoryType, TGroupId? groupId, string fileName);
	string GetWebFilePath(TDirectoryType directoryType, TGroupId? groupId, string fileName);
}