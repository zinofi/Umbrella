using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Legacy.WebUtilities.Middleware;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Caching.Options;

namespace Umbrella.Legacy.WebUtilities.Benchmark.Middleware
{
    [ClrJob]
    public class FrontEndCompressionMiddlewareBenchmark
    {
        private readonly FrontEndCompressionMiddleware _frontEndCompressionMiddleware;

        public FrontEndCompressionMiddlewareBenchmark()
        {
            // TODO
        }
    }
}