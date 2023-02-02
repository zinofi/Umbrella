// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

namespace Umbrella.FileSystem.Abstractions;

/// <summary>
/// A factory used to create instances of Umbrella File Storage Providers.
/// </summary>
public interface IUmbrellaFileStorageProviderFactory
{
	/// <summary>
	/// Creates a provider of the specified type using the specified options.
	/// </summary>
	/// <typeparam name="TProvider">The type of the provider.</typeparam>
	/// <typeparam name="TOptions">The type of the options.</typeparam>
	/// <param name="options">The options.</param>
	/// <returns>The file provider.</returns>
	TProvider CreateProvider<TProvider, TOptions>(TOptions options)
		where TProvider : IUmbrellaFileStorageProvider
		where TOptions : UmbrellaFileStorageProviderOptionsBase;
}