using Umbrella.Utilities.Threading.Abstractions;
using Umbrella.Utilities.Threading.Redis;
using Umbrella.Utilities.Threading.Redis.Options;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods used to register services for the <see cref="Umbrella.Utilities.Threading.Redis"/> package with a specified
/// <see cref="IServiceCollection"/> dependency injection container builder.
/// </summary>
public static class IServiceCollectionExtensions
{
	/// <summary>
	/// Adds the services required for <see cref="ISynchronizationManager"/> to use Redis cache as the locking mechanism.
	/// </summary>
	/// <param name="services">The services.</param>
	/// <param name="optionsBuilder">The options builder.</param>
	/// <returns>The services.</returns>
	public static IServiceCollection AddUmbrellaDistributedRedisLock(this IServiceCollection services, Action<IServiceProvider, DistributedRedisSynchronizationManagerOptions> optionsBuilder)
	{
		_ = services.ReplaceSingleton<ISynchronizationManager, DistributedRedisSynchronizationManager>();
		_ = services.ConfigureUmbrellaOptions(optionsBuilder);

		return services;
	}
}