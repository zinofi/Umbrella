// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Umbrella.FileSystem.Abstractions;
using Umbrella.FileSystem.Disk;
using Umbrella.Utilities;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods used to register services for the <see cref="Umbrella.FileSystem.Disk"/> package with a specified
	/// <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the <see cref="Umbrella.FileSystem.Disk"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <param name="optionsBuilder">The <see cref="UmbrellaDiskFileProviderOptions"/> builder.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="optionsBuilder"/> is null.</exception>
		public static IServiceCollection AddUmbrellaDiskFileProvider(this IServiceCollection services, Action<IServiceProvider, UmbrellaDiskFileProviderOptions> optionsBuilder)
			=> AddUmbrellaDiskFileProvider<UmbrellaDiskFileProvider>(services, optionsBuilder);

		/// <summary>
		/// Adds the <see cref="Umbrella.FileSystem.Disk"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <typeparam name="TFileProvider">
		/// The concrete implementation of <see cref="IUmbrellaDiskFileProvider"/> to register. This allows consuming applications to override the default implementation and allow it to be
		/// resolved from the container correctly for both the <see cref="IUmbrellaFileProvider"/> and <see cref="IUmbrellaDiskFileProvider"/> interfaces.
		/// </typeparam>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <param name="optionsBuilder">The <see cref="UmbrellaDiskFileProviderOptions"/> builder.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="optionsBuilder"/> is null.</exception>
		public static IServiceCollection AddUmbrellaDiskFileProvider<TFileProvider>(this IServiceCollection services, Action<IServiceProvider, UmbrellaDiskFileProviderOptions> optionsBuilder)
			where TFileProvider : class, IUmbrellaDiskFileProvider
			=> AddUmbrellaDiskFileProvider<TFileProvider, UmbrellaDiskFileProviderOptions>(services, optionsBuilder);

		/// <summary>
		/// Adds the <see cref="Umbrella.FileSystem.Disk"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <typeparam name="TFileProvider">
		/// The concrete implementation of <see cref="IUmbrellaDiskFileProvider"/> to register. This allows consuming applications to override the default implementation and allow it to be
		/// resolved from the container correctly for both the <see cref="IUmbrellaFileProvider"/> and <see cref="IUmbrellaDiskFileProvider"/> interfaces.
		/// </typeparam>
		/// <typeparam name="TOptions">The type of the options.</typeparam>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <param name="optionsBuilder">The <typeparamref name="TOptions"/> builder.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="optionsBuilder"/> is null.</exception>
		public static IServiceCollection AddUmbrellaDiskFileProvider<TFileProvider, TOptions>(this IServiceCollection services, Action<IServiceProvider, TOptions> optionsBuilder)
			where TFileProvider : class, IUmbrellaDiskFileProvider
			where TOptions : UmbrellaDiskFileProviderOptions, new()
		{
			Guard.IsNotNull(services, nameof(services));
			Guard.IsNotNull(optionsBuilder, nameof(optionsBuilder));

			services.AddUmbrellaFileSystemCore();

			services.AddSingleton<IUmbrellaDiskFileProvider>(x =>
			{
				var factory = x.GetRequiredService<IUmbrellaFileProviderFactory>();
				var options = x.GetRequiredService<TOptions>();

				return factory.CreateProvider<TFileProvider, TOptions>(options);
			});
			services.ReplaceSingleton<IUmbrellaFileProvider>(x => x.GetRequiredService<IUmbrellaDiskFileProvider>());

			// Options
			services.ConfigureUmbrellaOptions(optionsBuilder);

			return services;
		}
	}
}