using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.AspNetCore.WebUtilities.Mvc.TagHelpers;
using Xunit;

namespace Umbrella.AspNetCore.WebUtilities.Test.Mvc.TagHelpers
{
    public class ResponsiveImageTagHelperTest
    {
        [Theory]
        [InlineData(null, null, null)]
        [InlineData(null, "", null)]
        [InlineData("", null, null)]
        [InlineData(null, "1", null)]
        [InlineData(null, "2", null)]
        [InlineData(null, "2,3", null)]
        [InlineData("/path/to/image.png", "", null)]
        [InlineData("/path/to/image.png", "1", null)]
        [InlineData("/path/to/image.png", "2", "/path/to/image.png 1x, /path/to/image@2x.png 2x")]
        [InlineData("/path/to/image.png", "2,3", "/path/to/image.png 1x, /path/to/image@2x.png 2x, /path/to/image@3x.png 3x")]
        [InlineData("/path/to/image.png", "3", "/path/to/image.png 1x, /path/to/image@3x.png 3x")]
        [InlineData("/path/to/imagepng", "2", "Invalid image path")]
        [InlineData("http://www.google.com/path/to/image.png", "2", "http://www.google.com/path/to/image.png 1x, http://www.google.com/path/to/image@2x.png 2x")]
        [InlineData("https://www.google.com/path/to/image.png", "2", "https://www.google.com/path/to/image.png 1x, https://www.google.com/path/to/image@2x.png 2x")]
        public async Task Generate_Success(string path, string pixelDensities, string expectedOutput)
        {
            var tagHelper = CreateTagHelper();
            tagHelper.PixelDensities = pixelDensities;

            var ctx = Mocks.CreateTagHelperContext(new TagHelperAttributeList
            {
                new TagHelperAttribute("src", path),
                new TagHelperAttribute("alt", "hello"),
                new TagHelperAttribute("pixel-densities", pixelDensities)
            });

            var output = Mocks.CreateImageTagHelperOutput(new TagHelperAttributeList
            {
                new TagHelperAttribute("alt", "hello")
            }, "img");

            await tagHelper.ProcessAsync(ctx, output);

            Assert.True(output.Content.GetContent().Length == 0);
            Assert.Equal("img", output.TagName);

            bool srcSetShouldExist = !string.IsNullOrWhiteSpace(path)
                && !string.IsNullOrWhiteSpace(pixelDensities)
                && pixelDensities.Any(x => x >= '2' && x <= '9');
            
            int expectedAttributeCount = srcSetShouldExist ? 2 : 1;

            Assert.Equal(expectedAttributeCount, output.Attributes.Count);

            var srcSetAttribute = output.Attributes.SingleOrDefault(x => x.Name == "srcset");

            if (srcSetShouldExist)
            {
                Assert.NotNull(srcSetAttribute);
                Assert.Equal(expectedOutput, srcSetAttribute.Value);
            }
            else
                Assert.Null(srcSetAttribute);
        }

        private ResponsiveImageTagHelper CreateTagHelper()
            => new ResponsiveImageTagHelper(Mocks.CreateUmbrellaWebHostingEnvironment(), Mocks.CreateMemoryCache());
    }
}