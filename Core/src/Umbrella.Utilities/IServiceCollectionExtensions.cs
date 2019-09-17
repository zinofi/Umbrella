using System;
using System.Runtime.CompilerServices;
using Umbrella.Utilities;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Caching.Options;
using Umbrella.Utilities.DependencyInjection;
using Umbrella.Utilities.Email;
using Umbrella.Utilities.Email.Abstractions;
using Umbrella.Utilities.Email.Options;
using Umbrella.Utilities.Encryption;
using Umbrella.Utilities.Encryption.Abstractions;
using Umbrella.Utilities.Encryption.Options;
using Umbrella.Utilities.FriendlyUrl;
using Umbrella.Utilities.FriendlyUrl.Abstractions;
using Umbrella.Utilities.Hosting.Options;
using Umbrella.Utilities.Http;
using Umbrella.Utilities.Http.Abstractions;
using Umbrella.Utilities.Mime;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.Utilities.Numerics;
using Umbrella.Utilities.Numerics.Abstractions;
using Umbrella.Utilities.TypeConverters;
using Umbrella.Utilities.TypeConverters.Abstractions;

[assembly: InternalsVisibleTo("Umbrella.Utilities.Benchmark")]

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods used to register services for the <see cref="Umbrella.Utilities"/> package with a specified
	/// <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the <see cref="Umbrella.Utilities"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <param name="emailBuilderOptionsBuilder">The optional <see cref="EmailBuilderOptions"/> builder.</param>
		/// <param name="hybridCacheOptionsBuilder">The optional <see cref="HybridCacheOptions"/> builder.</param>
		/// <param name="httpResourceInfoUtilityOptionsBuilder">The optional <see cref="HttpResourceInfoUtilityOptions"/> builder.</param>
		/// <param name="secureRandomStringGeneratorOptionsBuilder">The optional <see cref="SecureRandomStringGeneratorOptions"/> builder.</param>
		/// <param name="umbrellaHostingEnvironmentOptionsBuilder">The optional <see cref="UmbrellaHostingEnvironmentOptions"/> builder.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		public static IServiceCollection AddUmbrellaUtilities(
			this IServiceCollection services,
			Action<IServiceProvider, EmailBuilderOptions> emailBuilderOptionsBuilder = null,
			Action<IServiceProvider, HybridCacheOptions> hybridCacheOptionsBuilder = null,
			Action<IServiceProvider, HttpResourceInfoUtilityOptions> httpResourceInfoUtilityOptionsBuilder = null,
			Action<IServiceProvider, SecureRandomStringGeneratorOptions> secureRandomStringGeneratorOptionsBuilder = null,
			Action<IServiceProvider, UmbrellaHostingEnvironmentOptions> umbrellaHostingEnvironmentOptionsBuilder = null)
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddSingleton<ICacheKeyUtility, CacheKeyUtility>();
			services.AddSingleton<ICertificateUtility, CertificateUtility>();
			services.AddSingleton<IConcurrentRandomGenerator, ConcurrentRandomGenerator>();
			services.AddSingleton<IFriendlyUrlGenerator, FriendlyUrlGenerator>();
			services.AddSingleton<IGenericTypeConverter, GenericTypeConverter>();
			services.AddSingleton<IHttpResourceInfoUtility, HttpResourceInfoUtility>();
			services.AddSingleton<IHybridCache, HybridCache>();
			services.AddSingleton<IMimeTypeUtility, MimeTypeUtility>();
			services.AddSingleton<INonceGenerator, NonceGenerator>();
			services.AddSingleton<ISecureRandomStringGenerator, SecureRandomStringGenerator>();
			services.AddTransient(typeof(Lazy<>), typeof(LazyProxy<>));
			services.AddTransient<IEmailBuilder, EmailBuilder>();

			if (hybridCacheOptionsBuilder == null)
			{
				// The HybridCacheOptions needs a default for the key builder which we need to create here
				// if no builder has been provided.
				services.AddSingleton(serviceProvider =>
				{
					ICacheKeyUtility cacheKeyUtility = serviceProvider.GetService<ICacheKeyUtility>();

					return new HybridCacheOptions
					{
						CacheKeyBuilder = (type, key) => cacheKeyUtility.Create(type, key)
					};
				});
			}
			else
			{
				services.ConfigureUmbrellaOptions(hybridCacheOptionsBuilder);
			}

			services.ConfigureUmbrellaOptions(emailBuilderOptionsBuilder);
			services.ConfigureUmbrellaOptions(httpResourceInfoUtilityOptionsBuilder);
			services.ConfigureUmbrellaOptions(secureRandomStringGeneratorOptionsBuilder);
			services.ConfigureUmbrellaOptions(umbrellaHostingEnvironmentOptionsBuilder);

			return services;
		}

		// TODO: Really need have an encryption utility options class with properties for key and iv and register with DI
		// It's a breaking change though so leave until v3.
		//public static IServiceCollection AddUmbrellaEncryptionUtility<T>(this IServiceCollection services, string encryptionKey, string iv)
		//	where T : class, IEncryptionUtility
		//{
		//	services.AddSingleton<IEncryptionUtility, T>

		//	return services;
		//}
	}
}