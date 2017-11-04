using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.AspNetCore.WebUtilities.Mvc.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Umbrella.WebUtilities.Hosting;
using Umbrella.DynamicImage.Abstractions;

namespace Umbrella.AspNetCore.DynamicImage.Mvc.TagHelpers
{
    [OutputElementHint("source")]
    [HtmlTargetElement("dynamic-source", Attributes = RequiredAttributeNames, ParentTag = "picture", TagStructure = TagStructure.WithoutEndTag)]
    public class DynamicImagePictureSourceTagHelper : DynamicImageTagHelperBase
    {
        protected override string OutputTagName => "source";

        public DynamicImagePictureSourceTagHelper(IMemoryCache memoryCache,
            IDynamicImageUtility dynamicImageUtility,
            IUmbrellaWebHostingEnvironment umbrellaHostingEnvironment)
            : base(memoryCache, dynamicImageUtility, umbrellaHostingEnvironment)
        {
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await base.ProcessAsync(context, output);

            output.Attributes.RemoveAll("alt");
            output.Attributes.RemoveAll("src");
        }
    }
}