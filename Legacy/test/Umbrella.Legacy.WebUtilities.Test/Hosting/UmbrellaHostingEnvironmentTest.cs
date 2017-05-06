using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Legacy.WebUtilities.Hosting;
using Xunit;

namespace Umbrella.Legacy.WebUtilities.Test.Hosting
{
    public class UmbrellaHostingEnvironmentTest
    {
        public static List<object[]> UrlsToTest = new List<object[]>
        {
            new object[] { "/path/to/a/resource.jpg" },
            new object[] { "path/to/a/resource.jpg" },
            new object[] { "~/path/to/a/resource.jpg" },
            new object[] { "//path////to/a/resource.jpg" }
        };

        [Theory]
        [MemberData(nameof(UrlsToTest))]
        public void TransformPath_EnsureStartsWithTildeSlash(string path)
        {
            var env = CreateHostingEnvironment();

            string transformedPath = env.TransformPath(path, true, false, false);

            Assert.Equal("~/path/to/a/resource.jpg", transformedPath);
        }

        [Theory]
        [MemberData(nameof(UrlsToTest))]
        public void TransformPath_VirtualPath_EnsureNoTilde_EnsureLeadingSlash(string path)
        {
            var env = CreateHostingEnvironment();

            string transformedPath = env.TransformPath(path, false, true, true);

            Assert.Equal("/path/to/a/resource.jpg", transformedPath);
        }

        private UmbrellaHostingEnvironment CreateHostingEnvironment()
        {
            var logger = new Mock<ILogger<UmbrellaHostingEnvironment>>();

            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

            return new UmbrellaHostingEnvironment(logger.Object, memoryCache);
        }
    }
}