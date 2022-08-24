// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Umbrella.AspNetCore.WebUtilities.FileSystem.Middleware;

#pragma warning disable IDE0130
namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods used to register Middleware for the <see cref="Umbrella.AspNetCore.WebUtilities.FileSystem"/> package with a specified <see cref="IApplicationBuilder"/>.
/// </summary>
public static class IApplicationBuilderExtensions
{
	/// <summary>
	/// Add the <see cref="FileSystemMiddleware"/> to the pipeline.
	/// </summary>
	/// <param name="builder">The builder.</param>
	/// <returns>The application builder.</returns>
	public static IApplicationBuilder UseUmbrellaFileSystem(this IApplicationBuilder builder)
	{
		Guard.IsNotNull(builder);

		_ = builder.UseMiddleware<FileSystemMiddleware>();

		return builder;
	}
}