using System.Runtime.CompilerServices;
using Umbrella.Utilities.Integration.NewtonsoftJson;

[assembly: InternalsVisibleTo("Umbrella.FileSystem.Test")]

namespace Microsoft.Extensions.DependencyInjection
{
	public static class IServiceCollectionExtensions
	{
		public static IServiceCollection AddUmbrellaUtilitiesNewtonsoftJson(this IServiceCollection services)
		{
			UmbrellaJsonIntegration.Initialize();

			return services;
		}
	}
}