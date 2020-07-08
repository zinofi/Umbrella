using System;
using System.Linq;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Options.Abstractions;
using Umbrella.WebUtilities.Exceptions;

namespace Umbrella.AspNetCore.WebUtilities.Security.Options
{
	/// <summary>
	/// Options for use with the <see cref="ApiIntegrationCookieAuthenticationEvents" /> type.
	/// </summary>
	/// <seealso cref="ISanitizableUmbrellaOptions" />
	/// <seealso cref="IValidatableUmbrellaOptions" />
	public class ApiIntegrationCookieAuthenticationEventsOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
	{
		/// <summary>
		/// Gets or sets the API path prefixes. Paths must begin with the leading forward slash '/'.
		/// This defaults to an array with a single item: "/api"
		/// </summary>
		public string[] ApiPathPrefixes { get; set; } = new[] { "/api" };

		/// <inheritdoc />
		public void Sanitize() => ApiPathPrefixes.Select(x => x.TrimToLowerInvariant()).SkipWhile(x => x is null).Distinct();

		/// <inheritdoc />
		public void Validate()
		{
			Guard.ArgumentNotNullOrEmpty(ApiPathPrefixes, nameof(ApiPathPrefixes));

			if (!ApiPathPrefixes.All(x => x.StartsWith('/')))
				throw new UmbrellaWebException("All paths must start with a leading forward slash, '/'.");
		}
	}
}