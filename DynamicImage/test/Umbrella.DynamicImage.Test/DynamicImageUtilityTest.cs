using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Umbrella.DynamicImage.Abstractions;

namespace Umbrella.DynamicImage.Test
{
    public class DynamicImageUtilityTest
    {
        [Fact]
        public void TryParseUrl()
        {
            var utility = CreateDynamicImageUtility();

            string path = "/dynamicimage/680/649/Uniform/png/images/mobile-devices@2x.jpg";

            var (status, imageOptions) = utility.TryParseUrl("dynamicimage", path);

            Assert.Equal(DynamicImageParseUrlResult.Success, status);

            var options = new DynamicImageOptions("/images/mobile-devices.png", 680 * 2, 649 * 2, DynamicResizeMode.Uniform, DynamicImageFormat.Jpeg);

            Assert.Equal(options, imageOptions);
        }

        [Fact]
        public void TryParseUrl_InvalidPathPrefix()
        {
            var utility = CreateDynamicImageUtility();

            string path = "/dynamicinvalidimage/680/649/Uniform/png/images/mobile-devices@2x.jpg";

            var (status, imageOptions) = utility.TryParseUrl("dynamicimage", path);

            Assert.Equal(DynamicImageParseUrlResult.Skip, status);
        }

        [Fact]
        public void TryParseUrl_InvalidPathSegmentOrder()
        {
            var utility = CreateDynamicImageUtility();

            string path = "/dynamicimage/images/649/Uniform/png/680/mobile-devices@2x.jpg";

            var (status, imageOptions) = utility.TryParseUrl("dynamicimage", path);

            Assert.Equal(DynamicImageParseUrlResult.Invalid, status);
        }

        [Fact]
        public void TryParseUrl_InvalidSegmentCount()
        {
            var utility = CreateDynamicImageUtility();

            string path = "/dynamicimage/649/Uniform/png/mobile-devices@2x.jpg";

            var (status, imageOptions) = utility.TryParseUrl("dynamicimage", path);

            Assert.Equal(DynamicImageParseUrlResult.Invalid, status);
        }

        private DynamicImageUtility CreateDynamicImageUtility()
        {
            var logger = new Mock<ILogger<DynamicImageUtility>>();

            return new DynamicImageUtility(logger.Object);
        }
    }
}