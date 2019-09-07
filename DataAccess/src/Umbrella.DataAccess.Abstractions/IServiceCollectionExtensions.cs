using Umbrella.DataAccess.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class IServiceCollectionExtensions
	{
		public static IServiceCollection AddUmbrellaDataAccess(this IServiceCollection services)
		{
			services.AddSingleton(typeof(ICurrentUserIdAccessor<>), typeof(DefaultUserIdAccessor<>));
			services.AddSingleton<IDataAccessLookupNormalizer, DataAccessUpperInvariantLookupNormalizer>();

			return services;
		}
	}
}