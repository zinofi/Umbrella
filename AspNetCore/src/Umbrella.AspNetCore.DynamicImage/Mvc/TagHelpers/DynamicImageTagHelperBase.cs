using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Umbrella.AspNetCore.WebUtilities.Mvc.TagHelpers;
using Umbrella.WebUtilities.Hosting;
using Umbrella.DynamicImage.Abstractions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Umbrella.Utilities;

namespace Umbrella.AspNetCore.DynamicImage.Mvc.TagHelpers
{
    public abstract class DynamicImageTagHelperBase : ResponsiveImageTagHelper
    {
        protected const string RequiredAttributeNames = "src," + WidthRequestAttributeName + "," + HeightRequestAttributeName + "," + ResizeModeAttributeName;

        protected const string WidthRequestAttributeName = "width-request";
        protected const string HeightRequestAttributeName = "height-request";
        protected const string ResizeModeAttributeName = "resize-mode";

        protected IDynamicImageUtility DynamicImageUtility { get; }
        protected abstract string OutputTagName { get; }

        /// <summary>
        /// This is the path prefix used for all generated image urls unless overridden using the "path-prefix" attribute on the tag.
        /// </summary>
        public static string GlobalDynamicImagePathPrefix { get; set; } = "dynamicimage";

        [HtmlAttributeName(WidthRequestAttributeName)]
        public int WidthRequest { get; set; }

        [HtmlAttributeName(HeightRequestAttributeName)]
        public int HeightRequest { get; set; }

        [HtmlAttributeName(ResizeModeAttributeName)]
        public DynamicResizeMode ResizeMode { get; set; }
        public DynamicImageFormat ImageFormat { get; set; } = DynamicImageFormat.Jpeg;
        

        [HtmlAttributeName("path-prefix")]
        public string DynamicImagePathPrefix { get; set; } = GlobalDynamicImagePathPrefix;

        public DynamicImageTagHelperBase(IMemoryCache memoryCache,
            IDynamicImageUtility dynamicImageUtility,
            IUmbrellaWebHostingEnvironment umbrellaHostingEnvironment)
            : base(umbrellaHostingEnvironment, memoryCache)
        {
            DynamicImageUtility = dynamicImageUtility;
        }

        public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            BuildCoreTag(output);

            await base.ProcessAsync(context, output);
        }

        protected string BuildCoreTag(TagHelperOutput output)
        {
            if (WidthRequest <= 0)
                throw new Exception($"The {WidthRequestAttributeName} value must be greater than 0.");

            if (HeightRequest <= 0)
                throw new Exception($"The {HeightRequestAttributeName} value must be greater than 0.");

            Guard.ArgumentNotNullOrWhiteSpace(DynamicImagePathPrefix, nameof(DynamicImagePathPrefix));

            TagHelperAttribute attrSrc = output.Attributes["src"];
            string src = attrSrc?.Value?.ToString();

            Guard.ArgumentNotNullOrWhiteSpace(src, nameof(src));

            var options = new DynamicImageOptions(src, WidthRequest, HeightRequest, ResizeMode, ImageFormat);

            string x1Url = DynamicImageUtility.GenerateVirtualPath(DynamicImagePathPrefix, options);

            output.Attributes.Remove(attrSrc);
            output.Attributes.Add("src", ResolveImageUrl(x1Url));

            output.TagName = OutputTagName;

            return src;
        }
    }
}