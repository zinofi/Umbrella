﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.AppFramework.Shared.Constants;

namespace Umbrella.AspNetCore.WebUtilities.Security
{
	/// <summary>
	/// An implementation of the <see cref="JwtBearerEvents" /> that allows file access authentication bearer tokens to be handled
	/// using the custom <see cref="AppAuthenticationSchemes.BearerFAT"/> authentication scheme.
	/// </summary>
	/// <seealso cref="JwtBearerEvents" />
	public class AppJwtBearerFATEvents : JwtBearerEvents
	{
		private const string TokenPrefix = AppAuthenticationSchemes.BearerFAT + " ";

		/// <inheritdoc />
		public override Task MessageReceived(MessageReceivedContext context)
		{
			string authorization = context.Request.Headers["Authorization"];

			if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith(TokenPrefix, StringComparison.OrdinalIgnoreCase))
			{
				context.Token = authorization.Substring(TokenPrefix.Length).Trim();
			}
			else if (context.Request.Query.TryGetValue(AppQueryStringKeys.FileAccessToken, out StringValues values))
			{
				context.Token = values.First().Trim();
			}

			// If no token found, no further work possible
			if (string.IsNullOrEmpty(context.Token))
			{
				context.NoResult();
				return Task.CompletedTask;
			}

			return Task.CompletedTask;
		}
	}
}