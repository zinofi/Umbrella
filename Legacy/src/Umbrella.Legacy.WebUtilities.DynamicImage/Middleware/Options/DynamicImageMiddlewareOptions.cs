using Umbrella.DynamicImage.Abstractions;
using Umbrella.FileSystem.Abstractions;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Middleware.Options
{
	// TODO: V3 - Consider moving to the common WebUtilities project
	public class DynamicImageMiddlewareOptions
	{
		public string CacheControlHeaderValue { get; set; } = "no-cache";
		public string DynamicImagePathPrefix { get; set; } = DynamicImageConstants.DefaultPathPrefix;
		public IUmbrellaFileProvider SourceFileProvider { get; set; }
		public bool EnableJpgPngWebPOverride { get; set; } // TODO: Consider defaulting to true. No good reason it shouldn't be.
	}
}