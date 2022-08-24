// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Umbrella.AppFramework.Http.Handlers;
using Umbrella.AppFramework.Http.Handlers.Options;
using Umbrella.AppFramework.Security;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.Utilities;
using Umbrella.AppFramework.Utilities.Abstractions;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods used to register services for the <see cref="Umbrella.AppFramework"/> package with a specified
/// <see cref="IServiceCollection"/> dependency injection container builder.
/// </summary>
public static class IServiceCollectionExtensions
{
	/// <summary>
	/// Adds the <see cref="Umbrella.AppFramework"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	/// <typeparam name="TAuthHelper">The type of the authentication helper.</typeparam>
	/// <param name="services">The services.</param>
	/// <returns>The services.</returns>
	public static IServiceCollection AddUmbrellaAppFramework<TAuthHelper>(this IServiceCollection services)
		where TAuthHelper : class, IAppAuthHelper
	{
		Guard.IsNotNull(services);

		_ = services.AddScoped<IAppAuthHelper, TAuthHelper>();

		// NB: These needs to be scoped for use with Blazor.
		_ = services.AddScoped<IAppUpdateMessageUtility, AppUpdateMessageUtility>();
		_ = services.AddScoped<IAppAuthTokenStorageService, AppAuthTokenStorageService>();

		_ = services.AddSingleton<IDialogTracker, DialogTracker>();
		_ = services.AddSingleton<ILoadingScreenUtility, LoadingScreenUtility>();

		// HTTP Delegating Handlers
		_ = services.AddScoped<AuthTokenHandler>();
		_ = services.AddScoped<RefreshedAuthTokenHandler>();
		_ = services.AddScoped<RequestNotificationHandler>();

		return services;
	}

	/// <summary>
	/// Configures the <see cref="Umbrella.AppFramework"/> services.
	/// </summary>
	/// <param name="services">The services.</param>
	/// <param name="requestNotificationHandlerOptionsBuilder">The request notification handler options builder.</param>
	/// <returns></returns>
	public static IServiceCollection ConfigureUmbrellaAppFramework(
		this IServiceCollection services,
		Action<IServiceProvider, RequestNotificationHandlerOptions>? requestNotificationHandlerOptionsBuilder = null)
	{
		// Options
		_ = services.ConfigureUmbrellaOptions(requestNotificationHandlerOptionsBuilder);

		return services;
	}
}