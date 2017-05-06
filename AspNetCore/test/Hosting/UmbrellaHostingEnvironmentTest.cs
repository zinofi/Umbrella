using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.AspNetCore.WebUtilities.Hosting;
using Xunit;

namespace Umbrella.AspNetCore.WebUtilities.Test.Hosting
{
    public class UmbrellaHostingEnvironmentTest
    {
        [Fact]
        public void MapPath_VirtualPath_ContentRoot()
        {
            string path = "~/images/test.jpg";

            var env = CreateHostingEnvironment();

            string mappedPath = env.MapPath(path);

            Assert.Equal(@"C:\MockedWebApp\src\images\test.jpg", mappedPath);
        }

        [Fact]
        public void MapPath_VirtualPath_WebRoot()
        {
            string path = "~/images/test.jpg";

            var env = CreateHostingEnvironment();

            string mappedPath = env.MapPath(path, false);

            Assert.Equal(@"C:\MockedWebApp\src\wwwroot\images\test.jpg", mappedPath);
        }

        [Fact]
        public void MapPath_RelativePath_ContentRoot()
        {
            string path = "/images/test.jpg";

            var env = CreateHostingEnvironment();

            string mappedPath = env.MapPath(path);

            Assert.Equal(@"C:\MockedWebApp\src\images\test.jpg", mappedPath);
        }

        [Fact]
        public void MapPath_RelativePath_WebRoot()
        {
            string path = "/images/test.jpg";

            var env = CreateHostingEnvironment();

            string mappedPath = env.MapPath(path, false);

            Assert.Equal(@"C:\MockedWebApp\src\wwwroot\images\test.jpg", mappedPath);
        }

        [Fact]
        public void MapWebPath_VirtualPath()
        {
            string path = "~/images/test.jpg";

            var env = CreateHostingEnvironment();

            string mappedPath = env.MapWebPath(path, true);

            Assert.Equal("http://www.test.com/images/test.jpg", mappedPath);
        }

        [Fact]
        public void MapWebPath_RelativePath()
        {
            string path = "/images/test.jpg";

            var env = CreateHostingEnvironment();

            string mappedPath = env.MapWebPath(path, true);

            Assert.Equal("http://www.test.com/images/test.jpg", mappedPath);
        }

        private UmbrellaHostingEnvironment CreateHostingEnvironment()
        {
            var logger = new Mock<ILogger<UmbrellaHostingEnvironment>>();

            var hostingEnvironment = new Mock<IHostingEnvironment>();
            hostingEnvironment.Setup(x => x.ContentRootPath).Returns(@"C:\MockedWebApp\src\");
            hostingEnvironment.Setup(x => x.WebRootPath).Returns(@"C:\MockedWebApp\src\wwwroot\");

            var httpContextAccessor = new Mock<IHttpContextAccessor>();

            var context = new DefaultHttpContext();
            context.Request.Host = new HostString("www.test.com");

            httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

            return new UmbrellaHostingEnvironment(logger.Object,
                hostingEnvironment.Object,
                httpContextAccessor.Object,
                memoryCache);
        }
    }
}