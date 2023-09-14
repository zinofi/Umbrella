namespace Umbrella.WebUtilities.Middleware.Options;

public class LinkHeaderMiddlewareOptions
{
	public HashSet<string> Urls { get; } = new(StringComparer.OrdinalIgnoreCase);
}