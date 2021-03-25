using System;
using Umbrella.AppFramework.Http.Handlers;
using Umbrella.AppFramework.Http.Handlers.Options;
using Umbrella.AppFramework.Security;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.Utilities;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
	/// <summary>
	/// Extension methods used to register services for the <see cref="Umbrella.AppFramework"/> package with a specified
	/// <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	public static class IServiceCollectionExtensions
    {
		/// <summary>
		/// Adds the <see cref="Umbrella.AppFramework"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
		/// </summary>
		/// /// <returns>The services builder.</returns>
		public static IServiceCollection AddUmbrellaAppFramework<TAuthHelper>(
			this IServiceCollection services,
			Action<IServiceProvider, RequestNotificationHandlerOptions>? requestNotificationHandlerOptionsBuilder = null)
			where TAuthHelper : class, IAppAuthHelper
		{
			Guard.ArgumentNotNull(services, nameof(services));

			services.AddScoped<IAppAuthHelper, TAuthHelper>();

			// NB: These needs to be scoped for use with Blazor.
			services.AddScoped<IAppUpdateMessageUtility, AppUpdateMessageUtility>();
			services.AddScoped<IAppAuthTokenStorageService, AppAuthTokenStorageService>();

			services.AddSingleton<IDialogTracker, DialogTracker>();
			services.AddSingleton<ILoadingScreenUtility, LoadingScreenUtility>();

			// HTTP Delegating Handlers
			services.AddScoped<AuthTokenHandler>();
			services.AddScoped<RefreshedAuthTokenHandler>();
			services.AddScoped<RequestNotificationHandler>();

			// Options
			services.ConfigureUmbrellaOptions(requestNotificationHandlerOptionsBuilder);

			return services;
		}
    }
}