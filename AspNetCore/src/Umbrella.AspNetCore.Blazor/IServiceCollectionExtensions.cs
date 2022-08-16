// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.AspNetCore.Blazor.Components.Dialog;
using Umbrella.AspNetCore.Blazor.Components.Dialog.Abstractions;
using Umbrella.AspNetCore.Blazor.Utilities;
using Umbrella.AspNetCore.Blazor.Utilities.Abstractions;
using Umbrella.Utilities;

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
	public static IServiceCollection AddUmbrellaBlazor(this IServiceCollection services)
	{
		Guard.ArgumentNotNull(services, nameof(services));

		_ = services.AddScoped<IAppLocalStorageService, BlazorLocalStorageService>();
		_ = services.AddScoped<IUmbrellaDialogUtility, UmbrellaDialogUtility>();
		_ = services.AddScoped<IUriNavigator, UriNavigator>();
		_ = services.AddTransient<IDialogUtility>(x => x.GetRequiredService<IUmbrellaDialogUtility>());
		_ = services.AddSingleton<IUmbrellaBlazorInteropUtility, UmbrellaBlazorInteropUtility>();

		return services;
	}
}