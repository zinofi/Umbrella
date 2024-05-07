using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.EntityFrameworkCore;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods used to register services for the <see cref="Umbrella.DataAccess.Abstractions"/> package with a specified
/// <see cref="IServiceCollection"/> dependency injection container builder.
/// </summary>
public static class IServiceCollectionExtensions
{
	/// <summary>
	/// Adds the <see cref="Umbrella.DataAccess.EntityFrameworkCore"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/> implementation to use.</typeparam>
	/// <param name="services">The services dependency injection container builder to which the services will be added.</param>
	/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
	public static IServiceCollection AddUmbrellaDataAccessEntityFrameworkCore<TDbContext>(this IServiceCollection services)
		where TDbContext : DbContext
	{
		Guard.IsNotNull(services);

		_ = services.AddScoped<IDataAccessUnitOfWork, DataAccessUnitOfWork<TDbContext>>();

		return services;
	}
}