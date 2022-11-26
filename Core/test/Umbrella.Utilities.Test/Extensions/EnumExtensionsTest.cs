using System;
using Umbrella.Utilities.Extensions;
using Xunit;

namespace Umbrella.Utilities.Test.Extensions;

public class EnumExtensionsTest
{
	[Flags]
	private enum TestType
	{
		Type1 = 0b0001,
		Type2 = 0b0010,
		Type3 = 0b0100,
		Type4 = 0b1000
	}

	[Fact]
	public void ToFlagsString_NullSeparator() => Assert.Throws<ArgumentNullException>(() => TestType.Type1.ToFlagsString(separator: null!));

	[Fact]
	public void ToFlagsString_Valid()
	{
		string result1 = (TestType.Type1 | TestType.Type2 | TestType.Type4).ToFlagsString();
		string result2 = (TestType.Type1 | TestType.Type4).ToFlagsString();
		string result3 = (TestType.Type1 | TestType.Type4).ToFlagsString(x => x.ToUpperInvariant());
		string result4 = (TestType.Type1 | TestType.Type4).ToFlagsString(separator: "+");

		Assert.Equal("Type1,Type2,Type4", result1);
		Assert.Equal("Type1,Type4", result2);
		Assert.Equal("TYPE1,TYPE4", result3);
		Assert.Equal("Type1+Type4", result4);
	}
}