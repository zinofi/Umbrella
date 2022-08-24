// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Umbrella.DynamicImage.Abstractions.Caching;
using Umbrella.DynamicImage.Caching.Disk;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods used to register services for the <see cref="Umbrella.DynamicImage.Caching.Disk"/> package with a specified
/// <see cref="IServiceCollection"/> dependency injection container builder.
/// </summary>
public static class IServiceCollectionExtensions
{
	/// <summary>
	/// Adds the <see cref="Umbrella.DynamicImage"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder
	/// with disk caching.
	/// </summary>
	/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
	/// <param name="dynamicImageCacheCoreOptionsBuilder">The optional <see cref="DynamicImageCacheCoreOptions"/> builder.</param>
	/// <param name="dynamicImageDiskCacheOptionsBuilder">The optional <see cref="DynamicImageDiskCacheOptions"/> builder.</param>
	/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
	public static IServiceCollection AddUmbrellaDynamicImageDiskCache(
		this IServiceCollection services,
		Action<IServiceProvider, DynamicImageCacheCoreOptions>? dynamicImageCacheCoreOptionsBuilder = null,
		Action<IServiceProvider, DynamicImageDiskCacheOptions>? dynamicImageDiskCacheOptionsBuilder = null)
	{
		Guard.IsNotNull(services);

		_ = services.AddUmbrellaDynamicImage();
		_ = services.ReplaceSingleton<IDynamicImageCache, DynamicImageDiskCache>();
		_ = services.ConfigureUmbrellaOptions(dynamicImageCacheCoreOptionsBuilder);
		_ = services.ConfigureUmbrellaOptions(dynamicImageDiskCacheOptionsBuilder);

		return services;
	}
}