// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.FileSystem.Abstractions;

/// <summary>
/// A handler used to access files stored using an implementation of the <see cref="UmbrellaFileProvider{TFileInfo, TOptions}"/>.
/// </summary>
/// <typeparam name="TGroupId">The type of the group identifier.</typeparam>
public interface IUmbrellaFileHandler<TGroupId>
	where TGroupId : IEquatable<TGroupId>
{
	Task<string> CreateByGroupIdAndTempFileNameAsync(TGroupId groupId, string tempFileName, CancellationToken cancellationToken = default);
	Task DeleteAllByGroupIdAsync(TGroupId groupId, CancellationToken cancellationToken = default);
	Task DeleteByGroupIdAndProviderFileNameAsync(TGroupId groupId, string providerFileName, CancellationToken cancellationToken = default);
	Task<string?> GetMostRecentUrlByGroupIdAsync(TGroupId groupId, CancellationToken cancellationToken = default);
	Task<string?> GetUrlByGroupIdAndProviderFileNameAsync(TGroupId groupId, string providerFileName, CancellationToken cancellationToken = default);
}