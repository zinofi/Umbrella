using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.Utilities.TypeConverters.Abstractions;

namespace Umbrella.FileSystem.Abstractions
{
	/// <summary>
	/// A factory used to create instances of Umbrella File Providers.
	/// </summary>
	/// <seealso cref="Umbrella.FileSystem.Abstractions.IUmbrellaFileProviderFactory" />
	public class UmbrellaFileProviderFactory : IUmbrellaFileProviderFactory
	{
		/// <summary>
		/// Gets the log.
		/// </summary>
		protected ILogger<UmbrellaFileProviderFactory> Log { get; }

		/// <summary>
		/// Gets the MIME type utility.
		/// </summary>
		protected IMimeTypeUtility MimeTypeUtility { get; }

		/// <summary>
		/// Gets the generic type converter.
		/// </summary>
		protected IGenericTypeConverter GenericTypeConverter { get; }

		/// <summary>
		/// Gets the DI services container.
		/// </summary>
		protected IServiceProvider Services { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaFileProviderFactory"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="mimeTypeUtility">The MIME type utility.</param>
		/// <param name="genericTypeConverter">The generic type converter.</param>
		/// <param name="services">The services.</param>
		public UmbrellaFileProviderFactory(
			ILogger<UmbrellaFileProviderFactory> logger,
			IMimeTypeUtility mimeTypeUtility,
			IGenericTypeConverter genericTypeConverter,
			IServiceProvider services)
		{
			Log = logger;
			MimeTypeUtility = mimeTypeUtility;
			GenericTypeConverter = genericTypeConverter;
			Services = services;
		}

		/// <inheritdoc />
		public TProvider CreateProvider<TProvider, TOptions>(TOptions options)
			where TProvider : IUmbrellaFileProvider
			where TOptions : IUmbrellaFileProviderOptions
		{
			var provider = ActivatorUtilities.CreateInstance<TProvider>(Services);
			provider.InitializeOptions(options);

			return provider;
		}
	}
}