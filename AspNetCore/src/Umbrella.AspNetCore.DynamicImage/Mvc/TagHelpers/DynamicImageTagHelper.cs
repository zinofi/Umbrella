using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Umbrella.AspNetCore.WebUtilities.Mvc.TagHelpers;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Utilities;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.AspNetCore.DynamicImage.Mvc.TagHelpers
{
    [OutputElementHint("img")]
    [HtmlTargetElement("dynamic-image", Attributes = RequiredAttributeNames, TagStructure = TagStructure.WithoutEndTag)]
    public class DynamicImageTagHelper : DynamicImageTagHelperBase
    {
        public string SizeWidths { get; set; }

        protected override string OutputTagName => "img";

        public DynamicImageTagHelper(IMemoryCache memoryCache,
            IDynamicImageUtility dynamicImageUtility,
            IUmbrellaWebHostingEnvironment umbrellaHostingEnvironment)
            : base(memoryCache, dynamicImageUtility, umbrellaHostingEnvironment)
        {
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            //If we don't have any size information just call into the base method
            var lstSizeWidth = GetParsedItems(SizeWidths);

            if (lstSizeWidth.Count == 0)
            {
                await base.ProcessAsync(context, output);
            }
            else
            {
                string src = BuildCoreTag(output);

                //TODO: Cache the srcSetValue using the src, PixelDensities and SizeWidths
                string srcsetValue = string.Join(", ", GetSizeStrings(src).Distinct().OrderBy(x => x));

                if (!string.IsNullOrWhiteSpace(srcsetValue))
                    output.Attributes.Add("srcset", srcsetValue);
            }
        }

        #region Private Methods
        private IReadOnlyCollection<int> GetSizeWidths() => GetParsedItems(SizeWidths);

        private IEnumerable<string> GetSizeStrings(string path)
        {
            float aspectRatio = WidthRequest / (float)HeightRequest;

            foreach (int sizeWidth in GetSizeWidths())
            {
                foreach (int density in GetPixelDensities())
                {
                    int imgWidth = sizeWidth * density;
                    int imgHeight = (int)Math.Ceiling(imgWidth / aspectRatio);

                    var options = new DynamicImageOptions(path, imgWidth, imgHeight, ResizeMode, ImageFormat);

                    string virtualPath = DynamicImageUtility.GenerateVirtualPath(DynamicImagePathPrefix, options);

                    string resolvedUrl = ResolveImageUrl(virtualPath);

                    yield return $"{resolvedUrl} {imgWidth}w";
                }
            }
        }
        #endregion
    }
}