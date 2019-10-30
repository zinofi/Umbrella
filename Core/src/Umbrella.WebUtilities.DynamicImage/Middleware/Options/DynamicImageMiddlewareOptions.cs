using Umbrella.DynamicImage.Abstractions;
using Umbrella.FileSystem.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.WebUtilities.DynamicImage.Middleware.Options
{
	// TODO: Alter the behaviour of this to work as per the FileProviderMiddlewareOptions
	// so that we only ever need one instance of the middleware in the pipeline
	// and the middleware can lookup the options it should be using with a key. The path prefix maybe??
	public class DynamicImageMiddlewareOptions : IValidatableUmbrellaOptions
	{
		public string CacheControlHeaderValue { get; set; } = "no-cache";
		public string DynamicImagePathPrefix { get; set; } = DynamicImageConstants.DefaultPathPrefix;
		public IUmbrellaFileProvider SourceFileProvider { get; set; }
		public bool EnableJpgPngWebPOverride { get; set; } = true;

		public void Validate()
		{
			Guard.ArgumentNotNull(SourceFileProvider, nameof(SourceFileProvider));
			Guard.ArgumentNotNullOrWhiteSpace(DynamicImagePathPrefix, nameof(DynamicImagePathPrefix));
		}
	}
}