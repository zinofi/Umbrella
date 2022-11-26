// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Umbrella.AppFramework.Shared.Constants;
using Umbrella.AspNetCore.WebUtilities.Middleware;
using Umbrella.DataAccess.Abstractions;

#pragma warning disable IDE0130
namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extentions methods for the <see cref="IApplicationBuilder" /> type.
/// These methods will usually be called when configuring the middleware pipeline in Startup.cs.
/// </summary>
public static class IApplicationBuilderExtensions
{
	/// <summary>
	/// Adds the <see cref="QueryStringParameterToHttpHeaderMiddleware" /> to the pipeline.
	/// </summary>
	/// <param name="builder">The builder.</param>
	/// <returns>The application builder.</returns>
	public static IApplicationBuilder UseUmbrellaQueryStringParameterToHttpHeader(this IApplicationBuilder builder) => builder.UseMiddleware<QueryStringParameterToHttpHeaderMiddleware>();

	/// <summary>
	/// Adds the <see cref="InternetExplorerCacheHeadersMiddleware" /> to the pipeline.
	/// </summary>
	/// <param name="builder">The builder.</param>
	/// <returns>The application builder.</returns>
	public static IApplicationBuilder UseUmbrellaInternetExplorerCacheHeaders(this IApplicationBuilder builder) => builder.UseMiddleware<InternetExplorerCacheHeadersMiddleware>();

	/// <summary>
	/// Adds the <see cref="MultiTenantSessionContextMiddleware{TAppTenantKey}"/> to the pipeline.
	/// </summary>
	/// <typeparam name="TAppTenantKey">The type of the application tenant key.</typeparam>
	/// <param name="builder">The builder.</param>
	/// <returns>The application builder.</returns>
	public static IApplicationBuilder UseUmbrellaMultiTenantSessionContext<TAppTenantKey>(this IApplicationBuilder builder) => builder.UseMiddleware<MultiTenantSessionContextMiddleware<TAppTenantKey>>();

	/// <summary>
	/// Adds the <see cref="MultiTenantSessionContextMiddleware{TAppTenantKey, TNullableAppTenantKey}"/> to the pipeline.
	/// </summary>
	/// <typeparam name="TAppTenantKey">The type of the application tenant key.</typeparam>
	/// <typeparam name="TNullableAppTenantKey">The type of the nullable application tenant key.</typeparam>
	/// <param name="builder">The builder.</param>
	/// <returns>The application builder.</returns>
	/// <remarks>
	/// The ability to allow a <typeparamref name="TNullableAppTenantKey"/> to be specified allows for a <see cref="DbAppTenantSessionContext{TNullableAppTenantKey}"/>
	/// to be accessed in a context where the current user may or may not be associated with a specific tenant. For example, application users will be stored in a database
	/// and each user will normally be associated with a tenant. However, the same database table may contain users not associated to a single tenant, e.g. system administrators,
	/// and those users will need to perform actions outside the context of a single tenant. The provision of a nullable key allows for this under these specialized circumstances.
	/// </remarks>
	public static IApplicationBuilder UseUmbrellaMultiTenantSessionContext<TAppTenantKey, TNullableAppTenantKey>(this IApplicationBuilder builder) => builder.UseMiddleware<MultiTenantSessionContextMiddleware<TAppTenantKey, TNullableAppTenantKey>>();

	/// <summary>
	/// Adds the <see cref="FrontEndCompressionMiddleware"/> to the pipeline.
	/// </summary>
	/// <param name="builder">The builder.</param>
	/// <returns>The application builder.</returns>
	public static IApplicationBuilder UseUmbrellaFrontEndCompression(this IApplicationBuilder builder) => builder.UseMiddleware<FrontEndCompressionMiddleware>();

	/// <summary>
	/// Adds the <see cref="ApiExceptionMiddleware"/> to the pipeline for all requests
	/// that have the specified <paramref name="pathPrefix"/>.
	/// </summary>
	/// <param name="builder">The builder.</param>
	/// <param name="pathPrefix">The path prefix for API endpoints.</param>
	/// <returns>The application builder.</returns>
	public static IApplicationBuilder UseUmbrellaApiException(this IApplicationBuilder builder, string pathPrefix = "/api") => builder.UseWhen(x => x.Request.Path.StartsWithSegments(pathPrefix, StringComparison.OrdinalIgnoreCase), app => app.UseMiddleware<ApiExceptionMiddleware>());

	/// <summary>
	/// Adds the <see cref="FileAccessTokenQueryStringMiddleware"/> to the pipeline for all requests
	/// that have a specified QueryString parameter with name <see cref="AppQueryStringKeys.FileAccessToken"/>.
	/// </summary>
	/// <param name="builder">The builder.</param>
	/// <returns>The application builder.</returns>
	public static IApplicationBuilder UseUmbrellaFileAccessTokenQueryString(this IApplicationBuilder builder) => builder.UseWhen(ctx => ctx.Request.Query.ContainsKey(AppQueryStringKeys.FileAccessToken), app => app.UseMiddleware<FileAccessTokenQueryStringMiddleware>());
}