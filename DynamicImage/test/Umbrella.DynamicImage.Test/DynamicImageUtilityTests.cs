using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Umbrella.DynamicImage;

namespace Umbrella.DynamicImage.Test
{
    public class DynamicImageUtilityTests
    {
        [Fact]
        public void TryParseUrl()
        {
            DynamicImageUtility utility = new DynamicImageUtility(null, null, null);

            var result = utility.TryParseUrl("dynamicimage", "/dynamicimage/680/649/Uniform/png/images/mobile-devices@2x.png");

            //TODO: Finish this off.
        }
    }
}
