using System;
using System.Runtime.CompilerServices;
using Umbrella.Utilities;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Caching.Options;
using Umbrella.Utilities.DependencyInjection;
using Umbrella.Utilities.Email;
using Umbrella.Utilities.Email.Interfaces;
using Umbrella.Utilities.Encryption;
using Umbrella.Utilities.Encryption.Abstractions;
using Umbrella.Utilities.Encryption.Options;
using Umbrella.Utilities.FriendlyUrl;
using Umbrella.Utilities.Http;
using Umbrella.Utilities.Mime;
using Umbrella.Utilities.Numerics;
using Umbrella.Utilities.Numerics.Abstractions;
using Umbrella.Utilities.TypeConverters;
using Umbrella.Utilities.TypeConverters.Abstractions;

[assembly: InternalsVisibleTo("Umbrella.Utilities.Benchmark")]

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods used to register services for the Umbrella.Utilities package with a specified
	/// <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the Umbrella.Utilities services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		public static IServiceCollection AddUmbrellaUtilities(this IServiceCollection services)
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddTransient(typeof(Lazy<>), typeof(LazyProxy<>));
			services.AddTransient<IEmailBuilder, EmailBuilder>();
			services.AddSingleton<IFriendlyUrlGenerator, FriendlyUrlGenerator>();
			services.AddSingleton<IMimeTypeUtility, MimeTypeUtility>();
			services.AddSingleton<ICacheKeyUtility, CacheKeyUtility>();
			services.AddSingleton<ICertificateUtility, CertificateUtility>();
			services.AddSingleton<ISecureRandomStringGenerator, SecureRandomStringGenerator>();
			services.AddSingleton<IMultiCache, MultiCache>();
			services.AddSingleton<INonceGenerator, NonceGenerator>();
			services.AddSingleton<IGenericTypeConverter, GenericTypeConverter>();
			services.AddSingleton<IHttpResourceInfoUtility, HttpResourceInfoUtility>();
			services.AddSingleton<IConcurrentRandomGenerator, ConcurrentRandomGenerator>();

			// Default Options - These can be replaced by calls to the Configure* methods below.
			services.AddSingleton(serviceProvider =>
			{
				var cacheKeyUtility = serviceProvider.GetService<ICacheKeyUtility>();

				return new MultiCacheOptions
				{
					CacheKeyBuilder = (type, key) => cacheKeyUtility.Create(type, key)
				};
			});
			services.AddSingleton<HttpResourceInfoUtilityOptions>();
			services.AddSingleton<SecureRandomStringGeneratorOptions>();

			return services;
		}

		/// <summary>
		/// Configures the <see cref="MultiCacheOptions"/> for use with the <see cref="MultiCache"/>.
		/// </summary>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <param name="optionsBuilder">An optional delegate used to build the <see cref="MultiCacheOptions"/>.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		public static IServiceCollection ConfigureMultiCacheOptions(this IServiceCollection services, Action<IServiceProvider, MultiCacheOptions> optionsBuilder)
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.ReplaceSingleton(serviceProvider =>
			{
				var cacheKeyUtility = serviceProvider.GetService<ICacheKeyUtility>();

				var options = new MultiCacheOptions
				{
					CacheKeyBuilder = (type, key) => cacheKeyUtility.Create(type, key)
				};

				// TODO V3: This should not be allowed to be null. Add a Guard check above and add an <exception> comment to the xml docs.
				optionsBuilder?.Invoke(serviceProvider, options);

				return options;
			});

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