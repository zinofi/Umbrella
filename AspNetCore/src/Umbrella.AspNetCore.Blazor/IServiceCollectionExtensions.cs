// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Components.Authorization;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.AspNetCore.Blazor.Components.Dialog;
using Umbrella.AspNetCore.Blazor.Components.Dialog.Abstractions;
using Umbrella.AspNetCore.Blazor.Security;
using Umbrella.AspNetCore.Blazor.Security.Abstractions;
using Umbrella.AspNetCore.Blazor.Security.Options;
using Umbrella.AspNetCore.Blazor.Services.Grid;
using Umbrella.AspNetCore.Blazor.Services.Grid.Abstractions;
using Umbrella.AspNetCore.Blazor.Utilities;
using Umbrella.AspNetCore.Blazor.Utilities.Abstractions;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods used to register services for the <see cref="Umbrella.AspNetCore.Blazor"/> package with a specified
/// <see cref="IServiceCollection"/> dependency injection container builder.
/// </summary>
public static class IServiceCollectionExtensions
{
	/// <summary>
	/// Adds the <see cref="Umbrella.AspNetCore.Blazor"/> services to the specified <see cref="IServiceCollection"/> dependency injection container builder.
	/// </summary>
	/// <returns>The services builder.</returns>
	public static IServiceCollection AddUmbrellaBlazor(
		this IServiceCollection services,
		Action<IServiceProvider, ClaimsPrincipalAuthenticationStateProviderOptions>? jwtAuthenticationStateProviderOptionsBuilder = null)
	{
		Guard.IsNotNull(services, nameof(services));

		_ = services.AddScoped<IAppLocalStorageService, BlazorLocalStorageService>();
		_ = services.AddScoped<IUmbrellaDialogUtility, UmbrellaDialogUtility>();
		_ = services.AddScoped<IUriNavigator, UriNavigator>();
		_ = services.AddTransient<IDialogUtility>(x => x.GetRequiredService<IUmbrellaDialogUtility>());
		_ = services.AddSingleton<IUmbrellaBlazorInteropUtility, UmbrellaBlazorInteropUtility>();
		_ = services.AddScoped<IUmbrellaGridComponentServiceFactory, UmbrellaGridComponentServiceFactory>();

		// Security
		_ = services.AddScoped<AuthenticationStateProvider, ClaimsPrincipalAuthenticationStateProvider>();
		_ = services.AddScoped<IClaimsPrincipalAuthenticationStateProvider>(x => (ClaimsPrincipalAuthenticationStateProvider)x.GetRequiredService<AuthenticationStateProvider>());
		_ = services.ConfigureUmbrellaOptions(jwtAuthenticationStateProviderOptionsBuilder);

		return services;
	}
}