using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.Utilities;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Adds the <see cref="Umbrella.AppFramework"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddUmbrellaAppFramework<TAuthHelper>(this IServiceCollection services)
			where TAuthHelper : class, IAppAuthHelper
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddScoped<IAppAuthHelper, TAuthHelper>();

			// NB: This needs to be scoped for use with Blazor.
			services.AddScoped<IAppUpdateMessageUtility, AppUpdateMessageUtility>();

			services.AddSingleton<IDialogTracker, DialogTracker>();
			services.AddSingleton<ILoadingScreenUtility, LoadingScreenUtility>();

			return services;
		}
    }
}