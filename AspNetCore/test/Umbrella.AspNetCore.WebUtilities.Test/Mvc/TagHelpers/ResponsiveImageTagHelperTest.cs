using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Umbrella.AspNetCore.WebUtilities.Razor.TagHelpers;
using Umbrella.Internal.Mocks;
using Xunit;

namespace Umbrella.AspNetCore.WebUtilities.Test.Mvc.TagHelpers;

public class ResponsiveImageTagHelperTest
{
	[Theory]
	[InlineData("", 1, null)]
	[InlineData(null, 1, null)]
	[InlineData(null, 2, null)]
	[InlineData(null, 3, null)]
	[InlineData("/path/to/image.png", 1, null)]
	[InlineData("/path/to/image.png", 2, "/path/to/image.png 1x, /path/to/image@2x.png 2x")]
	[InlineData("/path/to/image.png", 3, "/path/to/image.png 1x, /path/to/image@2x.png 2x, /path/to/image@3x.png 3x")]
	[InlineData("/path/to/imagepng", 2, "Invalid image path")]
	[InlineData("http://www.google.com/path/to/image.png", 2, "http://www.google.com/path/to/image.png 1x, http://www.google.com/path/to/image@2x.png 2x")]
	[InlineData("https://www.google.com/path/to/image.png", 2, "https://www.google.com/path/to/image.png 1x, https://www.google.com/path/to/image@2x.png 2x")]
	public async Task Generate_Success(string path, int maxPixelDensity, string expectedOutput)
	{
		var tagHelper = CreateTagHelper();
		tagHelper.MaxPixelDensity = maxPixelDensity;

		var ctx = Mocks.CreateTagHelperContext(new TagHelperAttributeList
		{
			new TagHelperAttribute("src", path),
			new TagHelperAttribute("alt", "hello"),
			new TagHelperAttribute("max-pixel-density", maxPixelDensity)
		});

		var output = Mocks.CreateImageTagHelperOutput(new TagHelperAttributeList
		{
			new TagHelperAttribute("alt", "hello")
		}, "img");

		await tagHelper.ProcessAsync(ctx, output);

		Assert.True(output.Content.GetContent().Length == 0);
		Assert.Equal("img", output.TagName);

		bool srcSetShouldExist = !string.IsNullOrWhiteSpace(path) && maxPixelDensity > 1;

		int expectedAttributeCount = srcSetShouldExist ? 2 : 1;

		Assert.Equal(expectedAttributeCount, output.Attributes.Count);

		var srcSetAttribute = output.Attributes.SingleOrDefault(x => x.Name == "srcset");

		if (srcSetShouldExist)
		{
			Assert.NotNull(srcSetAttribute);
			Assert.Equal(expectedOutput, srcSetAttribute!.Value);
		}
		else
		{
			Assert.Null(srcSetAttribute);
		}
	}

	private ResponsiveImageTagHelper CreateTagHelper()
		=> new ResponsiveImageTagHelper(
			CoreUtilitiesMocks.CreateLogger<ResponsiveImageTagHelper>(),
			Mocks.CreateUmbrellaWebHostingEnvironment(),
			CoreUtilitiesMocks.CreateHybridCache(),
			CoreUtilitiesMocks.CreateCacheKeyUtility(),
			CoreUtilitiesMocks.CreateResponsiveImageHelper());
}