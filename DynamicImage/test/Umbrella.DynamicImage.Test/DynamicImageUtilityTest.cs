using Microsoft.Extensions.Logging;
using Moq;
using Umbrella.DynamicImage.Abstractions;
using Xunit;

namespace Umbrella.DynamicImage.Test
{
	public class DynamicImageUtilityTest
	{
		[Fact]
		public void TryParseUrl()
		{
			DynamicImageUtility utility = CreateDynamicImageUtility();

			var path = "/dynamicimage/680/649/Uniform/png/images/mobile-devices@2x.jpg";

			(DynamicImageParseUrlResult status, DynamicImageOptions imageOptions) = utility.TryParseUrl(DynamicImageConstants.DefaultPathPrefix, path);

			Assert.Equal(DynamicImageParseUrlResult.Success, status);

			var options = new DynamicImageOptions("/images/mobile-devices.png", 680 * 2, 649 * 2, DynamicResizeMode.Uniform, DynamicImageFormat.Jpeg);

			Assert.Equal(options, imageOptions);
		}

		[Fact]
		public void TryParseUrl_InvalidPathPrefix()
		{
			DynamicImageUtility utility = CreateDynamicImageUtility();

			var path = "/dynamicinvalidimage/680/649/Uniform/png/images/mobile-devices@2x.jpg";
			(DynamicImageParseUrlResult status, _) = utility.TryParseUrl(DynamicImageConstants.DefaultPathPrefix, path);

			Assert.Equal(DynamicImageParseUrlResult.Skip, status);
		}

		[Fact]
		public void TryParseUrl_InvalidPathSegmentOrder()
		{
			DynamicImageUtility utility = CreateDynamicImageUtility();

			var path = "/dynamicimage/images/649/Uniform/png/680/mobile-devices@2x.jpg";
			(DynamicImageParseUrlResult status, _) = utility.TryParseUrl(DynamicImageConstants.DefaultPathPrefix, path);

			Assert.Equal(DynamicImageParseUrlResult.Invalid, status);
		}

		[Fact]
		public void TryParseUrl_InvalidSegmentCount()
		{
			DynamicImageUtility utility = CreateDynamicImageUtility();

			var path = "/dynamicimage/649/Uniform/png/mobile-devices@2x.jpg";
			(DynamicImageParseUrlResult status, _) = utility.TryParseUrl(DynamicImageConstants.DefaultPathPrefix, path);

			Assert.Equal(DynamicImageParseUrlResult.Invalid, status);
		}

		private DynamicImageUtility CreateDynamicImageUtility()
		{
			var logger = new Mock<ILogger<DynamicImageUtility>>();

			return new DynamicImageUtility(logger.Object);
		}
	}
}