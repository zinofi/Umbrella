using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.FileSystem.Abstractions;

namespace Umbrella.AspNetCore.DynamicImage.Middleware.Options
{
	// TODO: Re-port
    public class DynamicImageMiddlewareOptions
    {
        public string CacheControlHeaderValue { get; set; } = "no-cache";
		public string DynamicImagePathPrefix { get; set; } = DynamicImageConstants.DefaultPathPrefix;
        public IUmbrellaFileProvider SourceFileProvider { get; set; }
    }
}