﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Umbrella.AppFramework.Shared.Constants;
using Umbrella.AspNetCore.WebUtilities.Middleware.Options;

namespace Umbrella.AspNetCore.WebUtilities.Middleware;

/// <summary>
/// Middleware that intercepts requests that contain a QueryString parameter with
/// a name of <see cref="AppQueryStringKeys.FileAccessToken"/> and uses this to
/// create a <see cref="ClaimsPrincipal"/> if the value of the token is valid.
/// </summary>
public class FileAccessTokenQueryStringMiddleware
{
	private readonly ILogger _log;
	private readonly RequestDelegate _next;
	private readonly FileAccessTokenQueryStringMiddlewareOptions _options;

	/// <summary>
	/// Initializes a new instance of the <see cref="FileAccessTokenQueryStringMiddleware"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="next">The next.</param>
	/// <param name="options">The options.</param>
	public FileAccessTokenQueryStringMiddleware(
		ILogger<FileAccessTokenQueryStringMiddleware> logger,
		RequestDelegate next,
		FileAccessTokenQueryStringMiddlewareOptions options)
	{
		_log = logger;
		_next = next;
		_options = options;
	}

	/// <summary>
	/// Process an individual request.
	/// </summary>
	/// <param name="context">The current <see cref="HttpContext"/>.</param>
	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			string jwt = context.Request.Query[AppQueryStringKeys.FileAccessToken];

			JsonWebTokenHandler jwtHandler = new();

			TokenValidationResult validationResult = jwtHandler.ValidateToken(jwt, _options.ValidationParameters);

			if (validationResult.IsValid)
				context.User = new ClaimsPrincipal(validationResult.ClaimsIdentity);

			await _next(context);
		}
		catch (Exception exc) when (_log.WriteError(exc))
		{
			throw;
		}
	}
}