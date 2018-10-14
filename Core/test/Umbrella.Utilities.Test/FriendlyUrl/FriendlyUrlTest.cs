using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.FriendlyUrl;
using Xunit;

namespace Umbrella.Utilities.Test.FriendlyUrl
{
    public class FriendlyUrlTest
    {
        [Fact]
        public void TestOnlySpecialCharacters()
        {
            var generator = CreateFriendlyUrlGenerator();

            string url = generator.GenerateUrl("!\"£$%^&*()");

            Assert.Equal("", url);
        }

        [Fact]
        public void TestSpecialCharactersWith3ValidCharsAtStartMiddleAndEnd()
        {
            var generator = CreateFriendlyUrlGenerator();

            string url = generator.GenerateUrl("1!\"£$%f^&*()2");

            Assert.Equal("1-f-2", url);
        }

        [Fact]
        public void TestSpecialCharactersWith1ValidCharAtMiddle()
        {
            var generator = CreateFriendlyUrlGenerator();

            string url = generator.GenerateUrl("!\"£$%f^&*()");

            Assert.Equal("f", url);
        }

        private static FriendlyUrlGenerator CreateFriendlyUrlGenerator()
        {
            var loggerMock = new Mock<ILogger<FriendlyUrlGenerator>>();

            return new FriendlyUrlGenerator(loggerMock.Object);
        }
    }
}