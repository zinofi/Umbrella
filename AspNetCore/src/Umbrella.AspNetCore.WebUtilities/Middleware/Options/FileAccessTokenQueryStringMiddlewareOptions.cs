// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.IdentityModel.Tokens;
using Umbrella.Utilities;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Middleware.Options;

/// <summary>
/// Options for the FileAccessTokenQueryStringMiddleware.
/// </summary>
/// <seealso cref="ISanitizableUmbrellaOptions" />
/// <seealso cref="IValidatableUmbrellaOptions" />
public class FileAccessTokenQueryStringMiddlewareOptions : IValidatableUmbrellaOptions
{
	/// <summary>
	/// Gets or sets the validation parameters.
	/// </summary>
	public TokenValidationParameters ValidationParameters { get; set; } = null!;

	/// <inheritdoc/>
	public void Validate()
	{
		Guard.ArgumentNotNull(ValidationParameters, nameof(ValidationParameters));
	}
}