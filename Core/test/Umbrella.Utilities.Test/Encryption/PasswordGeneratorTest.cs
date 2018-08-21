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
        public static List<object[]> OptionsList = new List<object[]>
        {
            new object[] { 8, 3, 2 },
            new object[] { 16, 5, 3 },
            new object[] { 32, 14, 5 },
            new object[] { 32, 5, 14 },
            new object[] { 8, 0, 0 },
            new object[] { 8, 3, 0 },
            new object[] { 8, 0, 3 }
        };

        [Theory]
        [MemberData(nameof(OptionsList))]
        public void TestValidPassword(int length, int numbers, int upperCaseLetters)
        {
            var generator = CreatePasswordGenerator();

            int lowerCaseLetters = length - numbers - upperCaseLetters;

            string password = generator.GeneratePassword(length, numbers, upperCaseLetters);

            Assert.Equal(length, password.Length);

            int lowerCaseCount = password.Count(x => char.IsLower(x));
            int numbersCount = password.Count(x => char.IsNumber(x));
            int upperCaseCount = password.Count(x => char.IsUpper(x));

            Assert.Equal(lowerCaseLetters, lowerCaseCount);
            Assert.Equal(numbers, numbersCount);
            Assert.Equal(upperCaseLetters, upperCaseCount);
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

        [Fact]
        public void TestInvalidUpperCaseLetters()
        {
            var generator = CreatePasswordGenerator();

            Assert.Throws<ArgumentOutOfRangeException>(() => generator.GeneratePassword(upperCaseLetters: -1));
        }

        [Fact]
        public void TestUpperCaseLettersLessThanLength()
        {
            var generator = CreatePasswordGenerator();

            Assert.Throws<ArgumentOutOfRangeException>(() => generator.GeneratePassword(5, 1, 10));
        }

        [Fact]
        public void TestSumOfNumbersAndUpperCaseLettersLessThanLength()
        {
            var generator = CreatePasswordGenerator();

            Assert.Throws<ArgumentOutOfRangeException>(() => generator.GeneratePassword(10, 6, 6));
        }

        private IPasswordGenerator CreatePasswordGenerator()
        {
            var logger = new Mock<ILogger<PasswordGenerator>>();

            return new PasswordGenerator(logger.Object);
        }
    }
}