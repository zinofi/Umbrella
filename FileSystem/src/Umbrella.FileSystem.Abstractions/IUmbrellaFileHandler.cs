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
	/// <summary>
	/// Creates a new file by moving the specified temporary file from the temporary files directory to the top-level folder
	/// that this handler accesses inside a sub-folder specified by the <paramref name="groupId"/>.
	/// </summary>
	/// <param name="groupId">The group identifier.</param>
	/// <param name="tempFileName">Name of the temporary file to be moved.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The web-relative path the file can be accessed from a public URL.</returns>
	/// <remarks>
	/// Internally, handler implementations should do the following:
	/// <list type="bullet">
	/// <item>Check if the file already exists inside the target folder for this handler. If it does, just return the existing web relative path.</item>
	/// <item>Apply permissions to the moved file using an implemenation of the <see cref="IUmbrellaFileAccessUtility{TDirectoryType, TGroupId}.ApplyPermissionsAsync(IUmbrellaFileInfo, TGroupId, bool, CancellationToken)"/> method.</item>
	/// <item>Return the web relative path to the moved file.</item>
	/// </list>
	/// </remarks>
	Task<string> CreateByGroupIdAndTempFileNameAsync(TGroupId groupId, string tempFileName, CancellationToken cancellationToken = default);

	/// <summary>
	/// Deletes all files from the top-level folder for this handler contained within the sub-folder
	/// specified by the <paramref name="groupId"/>.
	/// </summary>
	/// <param name="groupId">The group identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task which completes when the files have been deleted.</returns>
	Task DeleteAllByGroupIdAsync(TGroupId groupId, CancellationToken cancellationToken = default);

	/// <summary>
	/// Deletes a single file specified by the <paramref name="providerFileName"/> contained within the sub-folder specified
	/// by the <paramref name="groupId"/> inside the top-level folder for this handler.
	/// </summary>
	/// <param name="groupId">The group identifier.</param>
	/// <param name="providerFileName">Name of the provider file.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A task which completes when the files have been deleted.</returns>
	Task DeleteByGroupIdAndProviderFileNameAsync(TGroupId groupId, string providerFileName, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the URL to the most recently created file inside the sub-folder specified by the <paramref name="groupId"/> within the top-level folder
	/// associated with this handler.
	/// </summary>
	/// <param name="groupId">The group identifier.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The web relative path to the file if one exists; otherwise <see langword="null"/>.</returns>
	Task<string?> GetMostRecentUrlByGroupIdAsync(TGroupId groupId, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the URL to the file with the specified <paramref name="providerFileName"/> contained with the sub-folder specified by <paramref name="groupId"/>
	/// within the top-level folder associated with this handler.
	/// </summary>
	/// <param name="groupId">The group identifier.</param>
	/// <param name="providerFileName">Name of the provider file.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>The web relative path to the file if one exists; otherwise <see langword="null"/>.</returns>
	Task<string?> GetUrlByGroupIdAndProviderFileNameAsync(TGroupId groupId, string providerFileName, CancellationToken cancellationToken = default);
}