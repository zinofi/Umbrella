using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.AspNetCore.DynamicImage.Middleware.Options
{
    public class DynamicImageMiddlewareOptions
    {
        public string DynamicImagePathPrefix { get; set; }
        public Func<string, Task<(byte[], DateTime)>> SourceImageResolver;
    }
}