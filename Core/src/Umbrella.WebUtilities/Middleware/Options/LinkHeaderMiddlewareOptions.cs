using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.WebUtilities.Middleware.Options;

/// <summary>
/// Options for use with associated middleware in ASP.NET and ASP.NET Core projects.
/// </summary>
public class LinkHeaderMiddlewareOptions : ISanitizableUmbrellaOptions
{
	/// <summary>
	/// The URLs to be output as <c>Link</c> headers.
	/// </summary>
	public HashSet<string> Urls { get; } = new(StringComparer.OrdinalIgnoreCase);

	/// <summary>
	/// Specifies whether the ingestion endpoints for Application Insights services
	/// should be included.
	/// </summary>
	/// <remarks>
	/// Defaults to <see langword="true" />
	/// </remarks>
	public bool AddApplicationInsightsEndpoints { get; set; } = true;

	/// <inheritdoc/>
	public void Sanitize()
	{
		HashSet<string> lstSanitizedUrl = new(Urls.Select(x => x.TrimToLowerInvariant()));

		if (AddApplicationInsightsEndpoints)
		{
			_ = lstSanitizedUrl.Add("https://js.monitor.azure.com");
			_ = lstSanitizedUrl.Add("https://dc.services.visualstudio.com");
		}

		Urls.Clear();
		Urls.AddRange(lstSanitizedUrl);
	}
}