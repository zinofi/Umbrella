using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.AspNetCore.WebUtilities.Hosting;
using Umbrella.Internal.Mocks;
using Umbrella.Utilities.Caching;
using Umbrella.Utilities.Data.Abstractions;
using Umbrella.Utilities.Hosting.Options;

namespace Umbrella.AspNetCore.WebUtilities.Test
{
    public static class Mocks
    {
        public static UmbrellaWebHostingEnvironment CreateUmbrellaWebHostingEnvironment()
        {
            var logger = new Mock<ILogger<UmbrellaWebHostingEnvironment>>();

            var hostingEnvironment = new Mock<IWebHostEnvironment>();
            hostingEnvironment.Setup(x => x.ContentRootPath).Returns(@"C:\MockedWebApp\src\");
            hostingEnvironment.Setup(x => x.WebRootPath).Returns(@"C:\MockedWebApp\src\wwwroot\");

            var httpContextAccessor = new Mock<IHttpContextAccessor>();

            var context = new DefaultHttpContext();
            context.Request.Host = new HostString("www.test.com");

            httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

			return new UmbrellaWebHostingEnvironment(logger.Object,
                hostingEnvironment.Object,
                httpContextAccessor.Object,
				new UmbrellaHostingEnvironmentOptions(),
                CreateMemoryCache(),
                CoreUtilitiesMocks.CreateCacheKeyUtility());
        }

        public static IMemoryCache CreateMemoryCache() => new MemoryCache(Options.Create(new MemoryCacheOptions()));

        public static TagHelperContext CreateTagHelperContext(TagHelperAttributeList attributes)
        {
            return new TagHelperContext(
                attributes,
                items: new Dictionary<object, object>(),
                uniqueId: Guid.NewGuid().ToString("N"));
        }

        public static TagHelperOutput CreateImageTagHelperOutput(TagHelperAttributeList attributes, string tagName)
        {
            attributes = attributes ?? new TagHelperAttributeList();

            return new TagHelperOutput(
                "img",
                attributes,
                getChildContentAsync: (useCachedResult, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetContent(default);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });
        }

        private static ViewContext CreateViewContext(string requestPathBase = null)
        {
            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            if (requestPathBase != null)
            {
                actionContext.HttpContext.Request.PathBase = new PathString(requestPathBase);
            }

            var metadataProvider = new EmptyModelMetadataProvider();
            var viewData = new ViewDataDictionary(metadataProvider, new ModelStateDictionary());
            var viewContext = new ViewContext(
                actionContext,
                Mock.Of<IView>(),
                viewData,
                Mock.Of<ITempDataDictionary>(),
                TextWriter.Null,
                new HtmlHelperOptions());

            return viewContext;
        }
    }
}