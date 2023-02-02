﻿using Umbrella.FileSystem.Abstractions;

namespace Umbrella.FileSystem.AzureStorage;

/// <summary>
/// This is a marker interface to allow the storage provider to be bound to both
/// the <see cref="IUmbrellaFileStorageProvider"/> and this <see cref="IUmbrellaAzureBlobFileStorageProvider"/> interface.
/// This would then allow multiple storage providers to be used in parallel in the same project.
/// </summary>
public interface IUmbrellaAzureBlobFileStorageProvider : IUmbrellaFileStorageProvider
{
	/// <summary>
	/// Clears the storage container resolution cache.
	/// </summary>
	void ClearContainerResolutionCache();
}