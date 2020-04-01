using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.Security.Options;
using Umbrella.WebUtilities.Exceptions;

namespace Umbrella.AspNetCore.WebUtilities.Security
{
	/// <summary>
	/// An customized version of the default <see cref="CookieAuthenticationEvents" /> type with customizations that intercept redirects
	/// that occur as a result of 401 and 403 status codes to ensure that API responses return the status codes instead of redirects to login or error pages which is the
	/// default behaviour. Non-API requests will fallback to the default redirect behaviour.
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents" />
	public class ApiIntegrationCookieAuthenticationEvents : CookieAuthenticationEvents
	{
		private readonly ILogger _logger;
		private readonly ApiIntegrationCookieAuthenticationEventsOptions _options;

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiIntegrationCookieAuthenticationEvents"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="options">The options.</param>
		public ApiIntegrationCookieAuthenticationEvents(
			ILogger<ApiIntegrationCookieAuthenticationEvents> logger,
			ApiIntegrationCookieAuthenticationEventsOptions options)
		{
			_logger = logger;
			_options = options;
		}

		/// <inheritdoc />
		public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
		{
			try
			{
				if (_options.ApiPathPrefixes.Any(x => context.Request.Path.StartsWithSegments(x)) && context.Response.StatusCode == StatusCodes.Status200OK)
				{
					context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				}
				else
				{
					context.Response.Redirect(context.RedirectUri);
				}

				return Task.CompletedTask;
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { context.Request.Path, context.Response.StatusCode }, returnValue: true))
			{
				throw new UmbrellaWebException("There was a problem handing the login redirect.", exc);
			}
		}

		/// <inheritdoc />
		public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
		{
			try
			{
				if (_options.ApiPathPrefixes.Any(x => context.Request.Path.StartsWithSegments(x)) && context.Response.StatusCode == StatusCodes.Status200OK)
				{
					context.Response.StatusCode = StatusCodes.Status403Forbidden;
				}
				else
				{
					context.Response.Redirect(context.RedirectUri);
				}

				return Task.CompletedTask;
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { context.Request.Path, context.Response.StatusCode }, returnValue: true))
			{
				throw new UmbrellaWebException("There was a problem handing the access denied redirect.", exc);
			}
		}
	}
}