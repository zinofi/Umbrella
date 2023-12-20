using Microsoft.AspNetCore.Http;
using Umbrella.AspNetCore.WebUtilities.Extensions;
using Xunit;

namespace Umbrella.AspNetCore.WebUtilities.Test.Extensions;

public class StringExtensionsTest
{
	public static TheoryData<string, string, int, string, string> ToAbsoluteUrlData = new()
	{
		{ "http", "localhost", 80, "~/path/to/foo", "http://localhost/path/to/foo" },
		{ "http", "localhost", 80, "/path/to/foo", "http://localhost/path/to/foo" },

		{ "https", "localhost", 443, "~/path/to/foo", "https://localhost/path/to/foo" },
		{ "https", "localhost", 443, "/path/to/foo", "https://localhost/path/to/foo" },

		{ "http", "localhost", 44332, "~/path/to/foo", "http://localhost:44332/path/to/foo" },
		{ "http", "localhost", 44332, "/path/to/foo", "http://localhost:44332/path/to/foo" },

		{ "https", "localhost", 44332, "~/path/to/foo", "https://localhost:44332/path/to/foo" },
		{ "https", "localhost", 44332, "/path/to/foo", "https://localhost:44332/path/to/foo" },
	};

	[Theory]
	[MemberData(nameof(ToAbsoluteUrlData))]
	public void ToAbsoluteUrl_ReturnsCorrectUrl(string scheme, string host, int port, string path, string expectedResult)
	{
		// Arrange
		var currentRequest = new DefaultHttpContext().Request;
		currentRequest.Scheme = scheme;
		currentRequest.Host = new HostString(host, port);

		// Act
		string result = path.ToAbsoluteUrl(currentRequest);

		// Assert
		Assert.Equal(expectedResult, result);
	}
}