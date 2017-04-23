using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.FileSystem.Abstractions;

namespace Umbrella.AspNetCore.DynamicImage.Middleware.Options
{
    public class DynamicImageMiddlewareOptions
    {
        public string DynamicImagePathPrefix { get; set; }
        public IUmbrellaFileProvider SourceFileProvider { get; set; }
    }
}