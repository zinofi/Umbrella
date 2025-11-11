// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Diagnostics;
using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.Mvc;
using Umbrella.WebUtilities.Versioning;
using Umbrella.WebUtilities.Versioning.Abstractions;
using Umbrella.WebUtilities.Versioning.Models;

#pragma warning disable IDE0130
namespace Microsoft.AspNetCore.Routing;

/// <summary>
/// Extensions for use with the <see cref="IEndpointRouteBuilder"/> type.
/// </summary>
public static class IEndpointRouteBuilderExtensions
{
	/// <summary>
	/// Adds a <see cref="RouteEndpoint"/> to the <see cref="IEndpointRouteBuilder"/> that matches HTTP GET requests
	/// for the specified <paramref name="path"/> which returns version information about the current system.
	/// </summary>
	/// <param name="builder">The builder.</param>
	/// <param name="path">The path.</param>
	/// <returns>A <see cref="RouteHandlerBuilder"/> that can be used to further customize the endpoint.</returns>
	public static RouteHandlerBuilder MapSystemVersionGet(this IEndpointRouteBuilder builder, string path = "/system-version")
		=> builder.MapGet(path, async (ILogger<SystemVersionService> logger, ISystemVersionService systemVersionService, CancellationToken cancellationToken) =>
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				SystemVersionModel model = await systemVersionService.GetAsync(cancellationToken).ConfigureAwait(false);

				return Results.Json(model);
			}
			catch (Exception exc) when (logger.WriteError(exc, new { path }))
			{
				return Results.Problem("There was a problem getting version information.", statusCode: 500);
			}
		});
}

/// <summary>
/// Extensions for the <see cref="IMvcBuilder"/> type.
/// </summary>
public static class IMvcBuilderExtensions
{
	/// <summary>
	/// Configures custom API behavior options for Umbrella, including a validation problem details response for invalid
	/// model states.
	/// </summary>
	/// <remarks>This method customizes the response returned when model validation fails, returning a problem
	/// details object with a status code of 400 or 422 depending on the model state. Use this method to ensure consistent
	/// validation error responses across your API.</remarks>
	/// <param name="builder">The MVC builder to configure. Cannot be null.</param>
	/// <returns>The same <see cref="IMvcBuilder"/> instance so that additional configuration calls can be chained.</returns>
	public static IMvcBuilder ConfigureUmbrellaApiBehaviorOptions(this IMvcBuilder builder)
	{
		Guard.IsNotNull(builder);

		_ = builder.ConfigureApiBehaviorOptions(options =>
		{
			options.InvalidModelStateResponseFactory = context =>
			{
				int statusCode = context.ModelState.ContainsKey("$") ? 400 : 422;

				var problemDetails = new UmbrellaValidationProblemDetails(context.ModelState)
				{
					Status = statusCode,
					TraceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
				};

				return new ObjectResult(problemDetails)
				{
					ContentTypes = { "application/problem+json" },
					StatusCode = statusCode
				};
			};
		});

		return builder;
	}
}