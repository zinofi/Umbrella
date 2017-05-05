using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Umbrella.DynamicImage;
using Moq;
using Microsoft.Extensions.Logging;

namespace Umbrella.DynamicImage.Test
{
    public class DynamicImageUtilityTest
    {
        [Fact]
        public void TryParseUrl()
        {
            //DynamicImageUtility utility = new DynamicImageUtility(null, null, null);

            //var result = utility.TryParseUrl("dynamicimage", "/dynamicimage/680/649/Uniform/png/images/mobile-devices@2x.jpg");

            //int i = 0;
            //TODO: Finish this off.
        }

        private DynamicImageUtility CreateDynamicImageUtility()
        {
            var logger = new Mock<ILogger<DynamicImageUtility>>();
            return null;
            //return new DynamicImageUtility(logger.Object, )
        }
    }
}
