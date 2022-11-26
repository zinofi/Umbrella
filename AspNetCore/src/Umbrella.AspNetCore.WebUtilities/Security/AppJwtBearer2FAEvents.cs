// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Umbrella.AspNetCore.WebUtilities.Security;

/// <summary>
/// An implementation of the <see cref="JwtBearerEvents" /> that allows Two-Factor authentication bearer tokens to be handled
/// using the custom <see cref="AppAuthenticationSchemes.Bearer2FA"/> authentication scheme.
/// </summary>
/// <seealso cref="JwtBearerEvents" />
public class AppJwtBearer2FAEvents : JwtBearerEvents
{
	private const string TokenPrefix = AppAuthenticationSchemes.Bearer2FA + " ";

	/// <inheritdoc />
	public override Task MessageReceived(MessageReceivedContext context)
	{
		string authorization = context.Request.Headers["Authorization"];

		// If no authorization header found, nothing to process further
		if (string.IsNullOrEmpty(authorization))
		{
			context.NoResult();
			return Task.CompletedTask;
		}

		if (authorization.StartsWith(TokenPrefix, StringComparison.OrdinalIgnoreCase))
			context.Token = authorization.Substring(TokenPrefix.Length).Trim();

		// If no token found, no further work possible
		if (string.IsNullOrEmpty(context.Token))
		{
			context.NoResult();
			return Task.CompletedTask;
		}

		return Task.CompletedTask;
	}
}