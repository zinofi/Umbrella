using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Middleware.Options
{
	// TODO: V3 - Consider moving to the common WebUtilities project
    public class DynamicImageMiddlewareOptions
    {
        public string CacheControlHeaderValue { get; set; } = "no-cache";
        public string DynamicImagePathPrefix { get; set; } = "dynamicimage";
        public IUmbrellaFileProvider SourceFileProvider { get; set; }
		public bool EnableJpgPngWebPOverride { get; set; }
	}
}