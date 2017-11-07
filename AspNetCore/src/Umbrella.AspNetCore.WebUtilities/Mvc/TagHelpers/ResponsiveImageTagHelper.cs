using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Html;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.TagHelpers
{
    [OutputElementHint("img")]
    [HtmlTargetElement("img", Attributes = RequiredAttributeNames)]
    public class ResponsiveImageTagHelper : TagHelper
    {
        private const string RequiredAttributeNames = "src," + PixelDensitiesAttributeName;

        protected const string PixelDensitiesAttributeName = "pixel-densities";

        #region Private Static Members
        private static readonly char[] s_SeparatorCharacterArray = new[] { ',' };
        #endregion

        #region Public Properties
        [HtmlAttributeName(PixelDensitiesAttributeName)]
        public string PixelDensities { get; set; } = "";
        #endregion

        protected virtual int MinPixelDensitiesRequiredToGenerateSrcsetAttribute => 1;

        #region Protected Properties
        protected IMemoryCache Cache { get; }
        protected IUmbrellaWebHostingEnvironment UmbrellaHostingEnvironment { get; }
        #endregion

        #region Constructors
        public ResponsiveImageTagHelper(IUmbrellaWebHostingEnvironment umbrellaHostingEnvironment,
            IMemoryCache memoryCache)
        {
            UmbrellaHostingEnvironment = umbrellaHostingEnvironment;
            Cache = memoryCache;
        }
        #endregion

        #region Overridden Methods
        public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await base.ProcessAsync(context, output);

            string path = output.Attributes["src"]?.Value as string ?? context.AllAttributes["src"]?.Value?.ToString();
            
            if (!string.IsNullOrWhiteSpace(path))
            {
                //Cache this using the image src attribute value and PixelDensities
                string srcsetValue = Cache.GetOrCreate(GetCacheKey(path, PixelDensities), entry =>
                {
                    entry.SetSlidingExpiration(TimeSpan.FromHours(1)).SetPriority(CacheItemPriority.Low);

                    //Cache this using PixelDensities as a key
                    var lstPixelDensity = GetPixelDensities();

                    if (lstPixelDensity.Count <= MinPixelDensitiesRequiredToGenerateSrcsetAttribute)
                        return string.Empty;

                    int densityIndex = path.LastIndexOf('.');

                    IEnumerable<string> srcsetImagePaths =
                        from density in lstPixelDensity
                        orderby density
                        let densityX = $"{density}x"
                        let highResImagePath = density > 1 ? path.Insert(densityIndex, $"@{densityX}") : path
                        select ResolveImageUrl(highResImagePath) + " " + densityX;

                    return string.Join(", ", srcsetImagePaths);
                });

                if (!string.IsNullOrWhiteSpace(srcsetValue))
                    output.Attributes.Add("srcset", srcsetValue);
            }
        }
        #endregion

        #region Protected Methods
        protected string ResolveImageUrl(string url)
        {
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return url;

            return UmbrellaHostingEnvironment.MapWebPath(url);
        }

        protected string GetCacheKey(params string[] keyItems) => $"{GetType().FullName}{string.Join(":", keyItems)}";

        protected IEnumerable<int> ParseIntegers(string[] items)
        {
            foreach (string item in items)
            {
                if (int.TryParse(item, out int value))
                    yield return value;
            }
        }

        protected IReadOnlyCollection<int> GetPixelDensities() => GetParsedItems(PixelDensities, 1);

        protected IReadOnlyCollection<int> GetParsedItems(string itemsString, params int[] initialItems)
        {
            return Cache.GetOrCreate<IReadOnlyCollection<int>>(GetCacheKey(itemsString), innerEntry =>
            {
                innerEntry.SetSlidingExpiration(TimeSpan.FromHours(1));

                if (string.IsNullOrWhiteSpace(itemsString))
                    return new List<int>();

                string[] items = itemsString.Split(s_SeparatorCharacterArray, StringSplitOptions.RemoveEmptyEntries);

                var set = new HashSet<int>(ParseIntegers(items));

                if (initialItems?.Length > 0)
                {
                    foreach (var item in initialItems)
                        set.Add(item);
                }

                return set;
            });
        }
        #endregion
    }
}