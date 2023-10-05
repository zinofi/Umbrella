using CommunityToolkit.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Umbrella.DataAccess.Abstractions;
using Umbrella.DataAccess.EntityFrameworkCore.Repositories;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods used to register services for the <see cref="Umbrella.DataAccess.EntityFrameworkCore.SqlServer"/> package with a specified
/// <see cref="IServiceCollection"/> dependency injection container builder.
/// </summary>
public static class IServiceCollectionExtensions
{
	/// <summary>
	/// Adds the <see cref="Umbrella.DataAccess.EntityFrameworkCore.SqlServer"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	/// <typeparam name="TDbContext">The type of the database context.</typeparam>
	/// <param name="services">The services.</param>
	/// <returns>The <see cref="IServiceCollection"/> dependency injection container builder.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="services"/> is null.</exception>
	public static IServiceCollection AddUmbrellaDataAccessEntityFrameworkCoreSqlServer<TDbContext>(this IServiceCollection services)
		where TDbContext : DbContext
	{
		Guard.IsNotNull(services);

		_ = services.AddScoped<IDatabaseVersionRepository, DatabaseVersionRepository<TDbContext>>();

		return services;
	}
}