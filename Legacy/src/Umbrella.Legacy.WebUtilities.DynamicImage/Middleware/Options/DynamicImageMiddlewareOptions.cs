using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Middleware.Options
{
    public class DynamicImageMiddlewareOptions
    {
        public string CacheControlHeaderValue { get; set; } = "no-cache";
        public string DynamicImagePathPrefix { get; set; }
        public IUmbrellaFileProvider SourceFileProvider { get; set; }
    }
}