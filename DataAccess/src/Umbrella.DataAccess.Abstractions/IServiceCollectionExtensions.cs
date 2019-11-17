using System;
using Umbrella.DataAccess.Abstractions;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods used to register services for the <see cref="Umbrella.DataAccess.Abstractions"/> package with a specified
	/// <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	public static class IServiceCollectionExtensions
	{
		/// <summary>
		/// Adds the <see cref="Umbrella.DataAccess.Abstractions"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		public static IServiceCollection AddUmbrellaDataAccess(this IServiceCollection services)
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddSingleton(typeof(ICurrentUserIdAccessor<>), typeof(DefaultUserIdAccessor<>));
			services.AddSingleton<ICurrentUserIdAccessor, DefaultUserIdAccessor>();

			return services;
		}

		/// <summary>
		/// Adds the <see cref="Umbrella.DataAccess.Abstractions"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// <typeparam name="TCurrentUserIdAccessor">The type of the user id accessor implementation which implements <see cref="ICurrentUserIdAccessor"/>.</typeparam>
		/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
		/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
		public static IServiceCollection AddUmbrellaDataAccess<TCurrentUserIdAccessor>(this IServiceCollection services)
			where TCurrentUserIdAccessor : class, ICurrentUserIdAccessor
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddSingleton<ICurrentUserIdAccessor, TCurrentUserIdAccessor>();
			services.AddSingleton<ICurrentUserIdAccessor<int>>(x => x.GetService<ICurrentUserIdAccessor>());

			return services;
		}
	}
}