using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Utilities.Encryption;
using Umbrella.Utilities.Encryption.Abstractions;
using Xunit;
using Moq;

namespace Umbrella.Utilities.Test.Encryption
{
    public class SecureStringGeneratorTest
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
        public void TestValidSecureString(int length, int numbers, int upperCaseLetters)
        {
            var generator = CreateSecureStringGenerator();

            int lowerCaseLetters = length - numbers - upperCaseLetters;

            string password = generator.Generate(length, numbers, upperCaseLetters);

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
            var generator = CreateSecureStringGenerator();

            Assert.Throws<ArgumentOutOfRangeException>(() => generator.Generate(0));
        }

        [Fact]
        public void TestInvalidNumbers()
        {
            var generator = CreateSecureStringGenerator();

            Assert.Throws<ArgumentOutOfRangeException>(() => generator.Generate(numbers: -1));
        }

        [Fact]
        public void TestNumbersLessThanLength()
        {
            var generator = CreateSecureStringGenerator();

            Assert.Throws<ArgumentOutOfRangeException>(() => generator.Generate(5, 6));
        }

        [Fact]
        public void TestInvalidUpperCaseLetters()
        {
            var generator = CreateSecureStringGenerator();

            Assert.Throws<ArgumentOutOfRangeException>(() => generator.Generate(upperCaseLetters: -1));
        }

        [Fact]
        public void TestUpperCaseLettersLessThanLength()
        {
            var generator = CreateSecureStringGenerator();

            Assert.Throws<ArgumentOutOfRangeException>(() => generator.Generate(5, 1, 10));
        }

        [Fact]
        public void TestSumOfNumbersAndUpperCaseLettersLessThanLength()
        {
            var generator = CreateSecureStringGenerator();

            Assert.Throws<ArgumentOutOfRangeException>(() => generator.Generate(10, 6, 6));
        }

        private ISecureStringGenerator CreateSecureStringGenerator()
        {
            var logger = new Mock<ILogger<SecureStringGenerator>>();

            return new SecureStringGenerator(logger.Object);
        }
    }
}