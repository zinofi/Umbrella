using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Components.Authorization;
using Umbrella.AspNetCore.Blazor.Security;
using Umbrella.AspNetCore.Blazor.Security.Abstractions;
using Umbrella.AspNetCore.Blazor.Security.Options;

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
	public static IServiceCollection AddUmbrellaBlazorSecurity(
		this IServiceCollection services,
		Action<IServiceProvider, ClaimsPrincipalAuthenticationStateProviderOptions>? jwtAuthenticationStateProviderOptionsBuilder = null)
	{
		Guard.IsNotNull(services);

		// Security
		_ = services.AddScoped<AuthenticationStateProvider, ClaimsPrincipalAuthenticationStateProvider>();
		_ = services.AddScoped<IClaimsPrincipalAuthenticationStateProvider>(x => (ClaimsPrincipalAuthenticationStateProvider)x.GetRequiredService<AuthenticationStateProvider>());
		_ = services.ConfigureUmbrellaOptions(jwtAuthenticationStateProviderOptionsBuilder);

		return services;
	}
}