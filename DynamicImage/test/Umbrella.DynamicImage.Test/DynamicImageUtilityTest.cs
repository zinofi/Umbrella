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

            var result = utility.TryParseUrl("dynamicimage", path);

            Assert.Equal(DynamicImageParseUrlResult.Success, result.Status);

            var options = new DynamicImageOptions("/images/mobile-devices.png", 680 * 2, 649 * 2, DynamicResizeMode.Uniform, DynamicImageFormat.Jpeg);

            Assert.Equal(options, result.ImageOptions);
        }

        private DynamicImageUtility CreateDynamicImageUtility()
        {
            var logger = new Mock<ILogger<DynamicImageUtility>>();

            return new DynamicImageUtility(logger.Object);
        }
    }
}