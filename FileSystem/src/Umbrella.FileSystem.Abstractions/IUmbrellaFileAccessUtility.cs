// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.FileSystem.Abstractions;

public interface IUmbrellaFileAccessUtility<TDirectoryType, TGroupId>
	where TDirectoryType : struct, Enum
{
	Task ApplyPermissionsAsync(IUmbrellaFileInfo fileInfo, TGroupId groupId, CancellationToken cancellationToken = default, bool writeChanges = true);
	Task<bool> CanAccessAsync(IUmbrellaFileInfo fileInfo, CancellationToken cancellationToken = default);
	string GetTempDirectoryName();
	string GetTempFilePath(string fileName);
	string GetTempWebFilePath(string fileName);
	bool IsTempFilePath(string fileName);
	string GetDirectoryName(TDirectoryType directoryType, TGroupId groupId);
	string GetFilePath(TDirectoryType directoryType, TGroupId groupId, string fileName);
	string GetWebFilePath(TDirectoryType directoryType, TGroupId groupId, string fileName);
}