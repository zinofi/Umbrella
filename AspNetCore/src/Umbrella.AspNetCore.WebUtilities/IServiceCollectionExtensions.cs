using System;
using Umbrella.AspNetCore.WebUtilities.Hosting;
using Umbrella.AspNetCore.WebUtilities.Mvc.Filters;
using Umbrella.AspNetCore.WebUtilities.Mvc.ModelState;
using Umbrella.Utilities;
using Umbrella.Utilities.Hosting;
using Umbrella.WebUtilities.Hosting;
using Umbrella.WebUtilities.ModelState;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods used to register services for the <see cref="Umbrella.AspNetCore.WebUtilities"/> package with a specified
	/// <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the <see cref="Umbrella.AspNetCore.WebUtilities"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		public static IServiceCollection AddUmbrellaAspNetCoreWebUtilities(this IServiceCollection services)
			=> services.AddUmbrellaAspNetCoreWebUtilities<UmbrellaWebHostingEnvironment>();

		/// <summary>
		/// Adds the <see cref="Umbrella.AspNetCore.WebUtilities"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <typeparam name="TUmbrellaWebHostingEnvironment">
		/// The concrete implementation of <see cref="IUmbrellaWebHostingEnvironment"/> to register. This allows consuming applications to override the default implementation and allow it to be
		/// resolved from the container correctly for both the <see cref="IUmbrellaHostingEnvironment"/> and <see cref="IUmbrellaWebHostingEnvironment"/> interfaces.
		/// </typeparam>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		public static IServiceCollection AddUmbrellaAspNetCoreWebUtilities<TUmbrellaWebHostingEnvironment>(this IServiceCollection services)
			where TUmbrellaWebHostingEnvironment : class, IUmbrellaWebHostingEnvironment
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddSingleton<ValidateModelStateAttribute>();

			// Add the hosting environment as a singleton and then ensure the same instance is bound to both interfaces
			services.AddSingleton<TUmbrellaWebHostingEnvironment>();
			services.AddSingleton<IUmbrellaHostingEnvironment>(x => x.GetService<TUmbrellaWebHostingEnvironment>());
			services.AddSingleton<IUmbrellaWebHostingEnvironment>(x => x.GetService<TUmbrellaWebHostingEnvironment>());

			// TODO: This ModelState stuff was added to try and standardize the error output from Web API endpoints. Need to look at the new "Problem Details" (RFC 7807)
			// stuff added in ASP.NET Core 2.1
			// Add the default ModelStateTransformer. This will need to replaced in consuming applications
			// where specific customizations need to be made to the ModelState and ModelStateEntry classes.
			// e.g. to apply TypeScript code generation attributes to each type.
			services.AddSingleton<IModelStateTransformer, ModelStateTransformer<DefaultTransformedModelState<DefaultTransformedModelStateEntry>, DefaultTransformedModelStateEntry>>();
			services.AddSingleton<ModelStateTransformerOptions>();

			return services;
		}
	}
}