using System.Web;
using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Extensions;

/// <summary>
/// Http Request Base Extensions
/// </summary>
public static class HttpRequestBaseExtensions
{
	/// <summary>
	/// Gets the ajax or fetch request header keys. Contains a single key named "X-Is-Ajax-Request" by default.
	/// </summary>
	/// <value>
	/// The ajax or fetch request header keys.
	/// </value>
	public static HashSet<string> AjaxOrFetchRequestHeaderKeys { get; } = new HashSet<string>
	{
		"X-Is-Ajax-Request"
	};

	/// <summary>
	/// Determines whether the request is an Ajax or Fetch request by checking the <see cref="AjaxOrFetchRequestHeaderKeys"/> collection and seeing
	/// if any of them are contained in the request headers. Header values are irrelevant. The presence of the header is what determines if this method
	/// returns true.
	/// <para>
	/// If none of the headers are found, the default MVC method <see cref="AjaxRequestExtensions.IsAjaxRequest(HttpRequestBase)"/> is used
	/// as a fallback.
	/// </para>
	/// </summary>
	/// <param name="request">The request.</param>
	/// <returns>
	/// <see langword="true"/> if the request satisfies the right conditions.
	/// </returns>
	public static bool IsAjaxOrFetchRequest(this HttpRequestBase request)
	{
		foreach (string key in AjaxOrFetchRequestHeaderKeys.Where(x => !string.IsNullOrWhiteSpace(x)))
		{
			if (bool.TryParse(request.Headers[key], out bool result))
			{
				return result;
			}
		}

		return request.IsAjaxRequest();
	}

	/// <summary>
	/// Determines whether the requesting client is IE by checking the User-Agent header to see if it contains
	/// the strings "MSIE" or "Trident" using ordinal case-insensitive comparison rules.
	/// </summary>
	/// <param name="request">The request.</param>
	public static bool IsInternetExplorer(this HttpRequestBase request)
	{
		string userAgent = request.Headers["User-Agent"];

		if (string.IsNullOrWhiteSpace(userAgent))
			return false;

		return userAgent.Contains("MSIE", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("Trident", StringComparison.OrdinalIgnoreCase);
	}
}