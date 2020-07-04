using Umbrella.AspNetCore.WebUtilities.Middleware;
using Umbrella.DataAccess.Abstractions;
using Umbrella.Utilities;

namespace Microsoft.AspNetCore.Builder
{
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
		public static IApplicationBuilder UseUmbrellaQueryStringParameterToHttpHeader(this IApplicationBuilder builder)
		{
			Guard.ArgumentNotNull(builder, nameof(builder));

			return builder.UseMiddleware<QueryStringParameterToHttpHeaderMiddleware>();
		}

		/// <summary>
		/// Adds the <see cref="InternetExplorerCacheHeadersMiddleware" /> to the pipeline.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <returns>The application builder.</returns>
		public static IApplicationBuilder UseUmbrellaInternetExplorerCacheHeaders(this IApplicationBuilder builder)
		{
			Guard.ArgumentNotNull(builder, nameof(builder));

			return builder.UseMiddleware<InternetExplorerCacheHeadersMiddleware>();
		}

		/// <summary>
		/// Adds the <see cref="MultiTenantSessionContextMiddleware{TAppTenantKey}"/> to the pipeline.
		/// </summary>
		/// <typeparam name="TAppTenantKey">The type of the application tenant key.</typeparam>
		/// <param name="builder">The builder.</param>
		/// <returns>The application builder.</returns>
		public static IApplicationBuilder UseUmbrellaMultiTenantSessionContext<TAppTenantKey>(this IApplicationBuilder builder)
		{
			Guard.ArgumentNotNull(builder, nameof(builder));

			return builder.UseMiddleware<MultiTenantSessionContextMiddleware<TAppTenantKey>>();
		}

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
		public static IApplicationBuilder UseUmbrellaMultiTenantSessionContext<TAppTenantKey, TNullableAppTenantKey>(this IApplicationBuilder builder)
		{
			Guard.ArgumentNotNull(builder, nameof(builder));

			return builder.UseMiddleware<MultiTenantSessionContextMiddleware<TAppTenantKey, TNullableAppTenantKey>>();
		}

		/// <summary>
		/// Add the <see cref="FrontEndCompressionMiddleware"/> to the pipeline.
		/// </summary>
		/// <param name="builder">The builder.</param>
		/// <returns>The application builder.</returns>
		public static IApplicationBuilder UseUmbrellaFrontEndCompression(this IApplicationBuilder builder)
		{
			Guard.ArgumentNotNull(builder, nameof(builder));

			builder.UseMiddleware<FrontEndCompressionMiddleware>();

			return builder;
		}
	}
}