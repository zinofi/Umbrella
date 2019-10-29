using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using Umbrella.Utilities.Encryption;
using Umbrella.Utilities.Encryption.Abstractions;
using Umbrella.Utilities.Encryption.Options;
using Xunit;

namespace Umbrella.Utilities.Test.Encryption
{
	public class SecureStringGeneratorTest
	{
		public static List<object[]> OptionsList = new List<object[]>
		{
			new object[] { 8, 3, 2, 0 },
			new object[] { 16, 5, 3, 0 },
			new object[] { 32, 14, 5, 0 },
			new object[] { 32, 5, 14, 0 },
			new object[] { 8, 0, 0, 0 },
			new object[] { 8, 3, 0, 0 },
			new object[] { 8, 0, 3, 0 },
			new object[] { 8, 3, 2, 1 },
			new object[] { 16, 5, 3, 2 },
			new object[] { 32, 14, 5, 3 },
			new object[] { 32, 5, 14, 4 },
			new object[] { 8, 0, 0, 5 },
			new object[] { 8, 3, 0, 2 },
			new object[] { 8, 0, 3, 3 }
		};

		private static readonly SecureRandomStringGeneratorOptions _options = new SecureRandomStringGeneratorOptions();

		[Theory]
		[MemberData(nameof(OptionsList))]
		public void TestValidSecureString(int length, int numbers, int upperCaseLetters, int specialCharacters)
		{
			var generator = CreateSecureStringGenerator();

			int lowerCaseLetters = length - numbers - upperCaseLetters - specialCharacters;

			string password = generator.Generate(length, numbers, upperCaseLetters, specialCharacters);

			Assert.Equal(length, password.Length);

			int lowerCaseCount = password.Count(x => char.IsLower(x));
			int numbersCount = password.Count(x => char.IsNumber(x));
			int upperCaseCount = password.Count(x => char.IsUpper(x));
			int specialCount = password.Count(x => _options.SpecialCharacters.Contains(x));

			Assert.Equal(lowerCaseLetters, lowerCaseCount);
			Assert.Equal(numbers, numbersCount);
			Assert.Equal(upperCaseLetters, upperCaseCount);
			Assert.Equal(specialCharacters, specialCount);
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

			Assert.Throws<ArgumentOutOfRangeException>(() => generator.Generate(upperCaseCharacters: -1));
		}

		[Fact]
		public void TestUpperCaseLettersLessThanLength()
		{
			var generator = CreateSecureStringGenerator();

			Assert.Throws<ArgumentOutOfRangeException>(() => generator.Generate(5, 1, 10));
		}

		[Fact]
		public void TestInvalidSpecialCharacters()
		{
			var generator = CreateSecureStringGenerator();

			Assert.Throws<ArgumentOutOfRangeException>(() => generator.Generate(specialCharacters: -1));
		}

		[Fact]
		public void TestSpecialCharactersLessThanLength()
		{
			var generator = CreateSecureStringGenerator();

			Assert.Throws<ArgumentOutOfRangeException>(() => generator.Generate(5, 1, 1, 10));
		}

		[Fact]
		public void TestSumOfParamsLessThanLength()
		{
			var generator = CreateSecureStringGenerator();

			Assert.Throws<ArgumentOutOfRangeException>(() => generator.Generate(10, 6, 6, 6));
		}

		private ISecureRandomStringGenerator CreateSecureStringGenerator()
		{
			var logger = new Mock<ILogger<SecureRandomStringGenerator>>();

			return new SecureRandomStringGenerator(logger.Object, _options);
		}
	}
}