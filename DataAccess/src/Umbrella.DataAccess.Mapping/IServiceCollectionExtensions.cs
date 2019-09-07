using Umbrella.DataAccess.Mapping;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class IServiceCollectionExtensions
	{
		public static IServiceCollection AddUmbrellaDataAccessMapping(this IServiceCollection services)
		{
			services.AddSingleton<IMappingUtility, MappingUtility>();

			return services;
		}
	}
}