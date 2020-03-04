using System.Collections.Generic;
using System.Linq;
using Umbrella.Utilities;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.WebUtilities.Middleware.Options
{
	/// <summary>
	/// Options for implementations of the InternetExplorerCacheHeaderMiddleware in the ASP.NET and ASP.NET Core projects.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.Options.Abstractions.ISanitizableUmbrellaOptions" />
	/// <seealso cref="Umbrella.Utilities.Options.Abstractions.IValidatableUmbrellaOptions" />
	public class InternetExplorerCacheHeaderMiddlewareOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
	{
		/// <summary>
		/// Keywords to search the User-Agent header value for.
		/// </summary>
		public List<string> UserAgentKeywords { get; set; } = new List<string> { "MSIE", "Trident" };

		/// <summary>
		/// HTTP Methods that the middleware will act on.
		/// </summary>
		public List<string> Methods { get; set; } = new List<string> { "GET", "HEAD" };

		/// <summary>
		/// Content-Type header values that the middleware will act on.
		/// </summary>
		public List<string> ContentTypes { get; set; } = new List<string> { "application/json" };

		/// <summary>
		/// Sanitizes this instance.
		/// </summary>
		public void Sanitize()
		{
			UserAgentKeywords = UserAgentKeywords.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct().ToList();
			Methods = Methods.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct().ToList();
			ContentTypes = ContentTypes.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct().ToList();
		}

		/// <summary>
		/// Validates this instance.
		/// </summary>
		public void Validate()
		{
			Guard.ArgumentNotNullOrEmpty(UserAgentKeywords, nameof(UserAgentKeywords));
			Guard.ArgumentNotNullOrEmpty(Methods, nameof(Methods));
			Guard.ArgumentNotNullOrEmpty(ContentTypes, nameof(ContentTypes));
		}
	}
}