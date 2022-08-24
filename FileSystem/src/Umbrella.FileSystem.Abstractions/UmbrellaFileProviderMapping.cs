﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.FileSystem.Abstractions;

/// <summary>
/// Specifies a mapping between a collection of application relative folder paths and a <see cref="IUmbrellaFileProvider"/> instance.
/// </summary>
/// <seealso cref="ISanitizableUmbrellaOptions" />
/// <seealso cref="IValidatableUmbrellaOptions" />
public class UmbrellaFileProviderMapping : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaFileProviderMapping"/> class.
	/// </summary>
	/// <param name="fileProvider">The file provider.</param>
	/// <param name="appRelativeFolderPaths">The application relative folder paths.</param>
	public UmbrellaFileProviderMapping(IUmbrellaFileProvider fileProvider, params string[] appRelativeFolderPaths)
	{
		FileProvider = fileProvider;
		AppRelativeFolderPaths = appRelativeFolderPaths;
	}

	/// <summary>
	/// Gets the file provider.
	/// </summary>
	public IUmbrellaFileProvider FileProvider { get; }

	/// <summary>
	/// Gets the application relative folder paths.
	/// </summary>
	public IReadOnlyCollection<string> AppRelativeFolderPaths { get; private set; }

	/// <summary>
	/// Sanitizes this instance.
	/// </summary>
	public void Sanitize()
	{
		if (AppRelativeFolderPaths != null)
		{
			// When sanitizing the paths, ensure they all have a trailing '/'.
			// This is to avoid an issue encountered when matching paths that start with the same name,
			// e.g. /folder-cool and /folder-coolio would both be matched. Adding a trailing '/' resolves this.
			AppRelativeFolderPaths = AppRelativeFolderPaths
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => $"/{x.TrimToLowerInvariant().Trim().Trim('/').TrimEnd('/')}/")
				.Distinct()
				.ToList()
				.AsReadOnly();
		}
	}

	/// <summary>
	/// Validates this instance.
	/// </summary>
	public void Validate()
	{
		Guard.IsNotNull(FileProvider);
		Guard.IsNotNull(AppRelativeFolderPaths);
		Guard.HasSizeGreaterThan(AppRelativeFolderPaths, 0);
	}
}