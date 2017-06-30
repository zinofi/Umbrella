using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Utilities.Encryption;
using Umbrella.Utilities.Encryption.Interfaces;
using Xunit;
using Moq;

namespace Umbrella.Utilities.Test.Encryption
{
    public class PasswordGeneratorTest
    {
        [Fact]
        public void TestValidPassword()
        {
            var generator = CreatePasswordGenerator();

            string password = generator.GeneratePassword(8, 3);

            Assert.Equal(8, password.Length);

            int numbersCount = password.Where(x => char.IsNumber(x)).Count();

            Assert.Equal(3, numbersCount);
        }

        [Fact]
        public void TestInvalidLength()
        {
            var generator = CreatePasswordGenerator();

            Assert.Throws<ArgumentOutOfRangeException>(() => generator.GeneratePassword(0));
        }

        [Fact]
        public void TestInvalidNumbers()
        {
            var generator = CreatePasswordGenerator();

            Assert.Throws<ArgumentOutOfRangeException>(() => generator.GeneratePassword(numbers: -1));
        }

        [Fact]
        public void TestNumbersLessThanLength()
        {
            var generator = CreatePasswordGenerator();

            Assert.Throws<ArgumentOutOfRangeException>(() => generator.GeneratePassword(5, 6));
        }

        private IPasswordGenerator CreatePasswordGenerator()
        {
            var logger = new Mock<ILogger<PasswordGenerator>>();

            return new PasswordGenerator(logger.Object);
        }
    }
}