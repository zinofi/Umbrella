using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Primitives;
using Umbrella.AppFramework.Shared.Constants;

namespace Umbrella.AspNetCore.WebUtilities.Security;

/// <summary>
/// An implementation of the <see cref="JwtBearerEvents" /> that allows file access authentication bearer tokens to be handled
/// using the custom <see cref="AppAuthenticationSchemes.BearerFAT"/> authentication scheme.
/// </summary>
/// <seealso cref="JwtBearerEvents" />
[Obsolete("This will be removed in a future version as it is no longer needed.")]
public class AppJwtBearerFATEvents : JwtBearerEvents
{
	private const string TokenPrefix = AppAuthenticationSchemes.BearerFAT + " ";

	/// <inheritdoc />
	public override Task MessageReceived(MessageReceivedContext context)
	{
		Guard.IsNotNull(context);

		string? authorization = context.Request.Headers["Authorization"];

		if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith(TokenPrefix, StringComparison.OrdinalIgnoreCase))
		{
			context.Token = authorization.Substring(TokenPrefix.Length).Trim();
		}
		else if (context.Request.Query.TryGetValue(AppQueryStringKeys.FileAccessToken, out StringValues values))
		{
			context.Token = values.First()?.Trim();
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