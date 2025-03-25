using CommunityToolkit.Diagnostics;
using Microsoft.AspNetCore.Http;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Razor.Options;

/// <summary>
/// Options for the <see cref="RazorViewToStringRenderer"/>.
/// </summary>
public class RazorViewToStringRendererOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
{
	/// <summary>
	/// The Http scheme to use when rendering the view.
	/// </summary>
	/// <remarks>Defaults to <c>https</c></remarks>
	public string Scheme { get; set; } = "https";

	/// <summary>
	/// The host to use when rendering the view.
	/// </summary>
	public string? Host { get; set; }

	/// <summary>
	/// The request path to use when rendering the view.
	/// </summary>
	public string? RequestPath { get; set; }

	/// <summary>
	/// The request method to use when rendering the view.
	/// </summary>
	public string? RequestMethod { get; set; }

	/// <summary>
	/// Creates a new <see cref="HttpContext"/> instance based on the property values specified in this instance.
	/// </summary>
	/// <returns>The <see cref="HttpContext"/> instance.</returns>
	public HttpContext CreateHttpContext(IServiceProvider serviceProvider)
	{
		DefaultHttpContext httpContext = new()
		{
			RequestServices = serviceProvider
		};

		httpContext.Request.Scheme = Scheme;

		if (!string.IsNullOrEmpty(Host))
			httpContext.Request.Host = new HostString(Host);

		if (!string.IsNullOrEmpty(RequestPath))
			httpContext.Request.Path = RequestPath;

		if (!string.IsNullOrEmpty(RequestMethod))
			httpContext.Request.Method = RequestMethod;

		return httpContext;
	}

	/// <inheritdoc/>
	public void Sanitize()
	{
		Scheme = Scheme.TrimToLowerInvariant();
		Host = Host?.TrimToLowerInvariant();
		RequestPath = RequestPath?.TrimToLowerInvariant();
		RequestMethod = RequestMethod?.TrimToUpperInvariant();
	}

	/// <inheritdoc/>
	public void Validate()
	{
		Guard.IsNotNullOrEmpty(Scheme);
	}
}