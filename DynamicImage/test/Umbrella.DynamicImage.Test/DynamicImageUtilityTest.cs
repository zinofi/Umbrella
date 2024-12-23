﻿using Microsoft.Extensions.Logging;
using Moq;
using Umbrella.DynamicImage.Abstractions;
using Xunit;

namespace Umbrella.DynamicImage.Test;

public class DynamicImageUtilityTest
{
	[Theory]
	[InlineData(".png", DynamicImageFormat.Png)]
	[InlineData(".bmp", DynamicImageFormat.Bmp)]
	[InlineData(".jpg", DynamicImageFormat.Jpeg)]
	[InlineData(".gif", DynamicImageFormat.Gif)]
	[InlineData(".webp", DynamicImageFormat.WebP)]
	[InlineData(".avif", DynamicImageFormat.Avif)]
	[InlineData(" png", DynamicImageFormat.Png)]
	[InlineData(" bmp", DynamicImageFormat.Bmp)]
	[InlineData(" jpg", DynamicImageFormat.Jpeg)]
	[InlineData(" gif", DynamicImageFormat.Gif)]
	[InlineData(" avif", DynamicImageFormat.Avif)]
	public void ParseImageFormat(string format, DynamicImageFormat output)
	{
		DynamicImageUtility utility = CreateDynamicImageUtility();

		DynamicImageFormat result = utility.ParseImageFormat(format);

		Assert.Equal(output, result);
	}

	[Fact]
	public void TryParseUrl_Valid()
	{
		DynamicImageUtility utility = CreateDynamicImageUtility();

		string path = "/dynamicimage/680/649/Uniform/png/images/mobile-devices@2x.jpg";

		(DynamicImageParseUrlResult status, DynamicImageOptions imageOptions) = utility.TryParseUrl(DynamicImageConstants.DefaultPathPrefix, path);

		Assert.Equal(DynamicImageParseUrlResult.Success, status);

		var options = new DynamicImageOptions("/images/mobile-devices.png", 680 * 2, 649 * 2, DynamicResizeMode.Uniform, DynamicImageFormat.Jpeg);

		Assert.Equal(options, imageOptions);
	}

	[Fact]
	public void TryParseUrl_Valid_QueryString()
	{
		DynamicImageUtility utility = CreateDynamicImageUtility();

		string path = "/dynamicimage/680/649/Uniform/png/images/mobile-devices@2x.jpg?key=value.test.onex&stuff=xxx";

		(DynamicImageParseUrlResult status, DynamicImageOptions imageOptions) = utility.TryParseUrl(DynamicImageConstants.DefaultPathPrefix, path);

		Assert.Equal(DynamicImageParseUrlResult.Success, status);

		var options = new DynamicImageOptions("/images/mobile-devices.png", 680 * 2, 649 * 2, DynamicResizeMode.Uniform, DynamicImageFormat.Jpeg);

		Assert.Equal(options, imageOptions);
	}

	[Fact]
	public void TryParseUrl_InvalidPathPrefix()
	{
		DynamicImageUtility utility = CreateDynamicImageUtility();

		string path = "/dynamicinvalidimage/680/649/Uniform/png/images/mobile-devices@2x.jpg";
		(DynamicImageParseUrlResult status, _) = utility.TryParseUrl(DynamicImageConstants.DefaultPathPrefix, path);

		Assert.Equal(DynamicImageParseUrlResult.Skip, status);
	}

	[Fact]
	public void TryParseUrl_InvalidPathSegmentOrder()
	{
		DynamicImageUtility utility = CreateDynamicImageUtility();

		string path = "/dynamicimage/images/649/Uniform/png/680/mobile-devices@2x.jpg";
		(DynamicImageParseUrlResult status, _) = utility.TryParseUrl(DynamicImageConstants.DefaultPathPrefix, path);

		Assert.Equal(DynamicImageParseUrlResult.Invalid, status);
	}

	[Fact]
	public void TryParseUrl_InvalidSegmentCount()
	{
		DynamicImageUtility utility = CreateDynamicImageUtility();

		string path = "/dynamicimage/649/Uniform/png/mobile-devices@2x.jpg";
		(DynamicImageParseUrlResult status, _) = utility.TryParseUrl(DynamicImageConstants.DefaultPathPrefix, path);

		Assert.Equal(DynamicImageParseUrlResult.Invalid, status);
	}

	[Fact]
	public void TryParseUrl_MissingExtension()
	{
		DynamicImageUtility utility = CreateDynamicImageUtility();

		string path = "/dynamicimage/649/Uniform/png/mobile-devices@2x";
		(DynamicImageParseUrlResult status, _) = utility.TryParseUrl(DynamicImageConstants.DefaultPathPrefix, path);

		Assert.Equal(DynamicImageParseUrlResult.Invalid, status);
	}

	[Fact]
	public void GenerateVirtualPath_Valid()
	{
		DynamicImageUtility utility = CreateDynamicImageUtility();

		string url = utility.GenerateVirtualPath(DynamicImageConstants.DefaultPathPrefix, new DynamicImageOptions("/images/test.png", 100, 200, DynamicResizeMode.UniformFill, DynamicImageFormat.Jpeg));

		Assert.Equal($"~/{DynamicImageConstants.DefaultPathPrefix}/100/200/UniformFill/png/images/test.jpg", url);
	}

	[Fact]
	public void GenerateVirtualPath_Valid_QueryString()
	{
		DynamicImageUtility utility = CreateDynamicImageUtility();

		string url = utility.GenerateVirtualPath(DynamicImageConstants.DefaultPathPrefix, new DynamicImageOptions("/images/test.png?key=value.test.onEX&stuff=xxx", 100, 200, DynamicResizeMode.UniformFill, DynamicImageFormat.Jpeg));

		Assert.Equal($"~/{DynamicImageConstants.DefaultPathPrefix}/100/200/UniformFill/png/images/test.jpg?key=value.test.onEX&stuff=xxx", url);
	}

	private static DynamicImageUtility CreateDynamicImageUtility()
	{
		var logger = new Mock<ILogger<DynamicImageUtility>>();

		return new DynamicImageUtility(logger.Object);
	}
}