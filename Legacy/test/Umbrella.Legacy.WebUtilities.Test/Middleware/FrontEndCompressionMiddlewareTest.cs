﻿// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Owin;
using Moq;
using System.Text;
using Umbrella.Internal.Mocks;
using Umbrella.Legacy.WebUtilities.Middleware;
using Umbrella.Utilities.Caching.Abstractions;
using Umbrella.Utilities.Mime.Abstractions;
using Umbrella.WebUtilities.Hosting;
using Umbrella.WebUtilities.Http.Abstractions;
using Umbrella.WebUtilities.Middleware.Options;

namespace Umbrella.Legacy.WebUtilities.Test.Middleware;

public class FrontEndCompressionMiddlewareTest
{
	private const string DummyETagHeaderValue = nameof(DummyETagHeaderValue);
	private const string DummyLastModifiedHeaderValue = nameof(DummyLastModifiedHeaderValue);

	private static readonly byte[] _testContent = Encoding.UTF8.GetBytes("console.log(\"This is a test bit of JS!!\");");
	private static readonly DateTime _startJan2019Date = new(2019, 1, 1, 1, 1, 1);

	// [Fact]
	public async Task Invoke_Js_Valid()
	{
		var middleware = CreateMiddleware();

		var context = CreateOwinContext();
		context.Request.Path = new PathString("/sitefiles/exists.js");
		context.Request.Headers.Set("Accept-Encoding", "br, gzip, deflate");

		await middleware.Invoke(context);

		// TODO - vUnlikely: Check the response on the context
	}

	private IOwinContext CreateOwinContext()
	{
		var context = new Mock<IOwinContext>();

		// Request
		var request = new Mock<IOwinRequest>();
		_ = request.SetupAllProperties();
		_ = request.Setup(x => x.Headers).Returns(new HeaderDictionary(new Dictionary<string, string[]>()));

		_ = context.Setup(x => x.Request).Returns(request.Object);

		// Response
		var response = new Mock<IOwinResponse>();
		_ = response.Setup(x => x.Body).Returns(new MemoryStream());
		_ = response.Setup(x => x.Headers).Returns(new HeaderDictionary(new Dictionary<string, string[]>()));

		_ = context.Setup(x => x.Response).Returns(response.Object);

		return context.Object;
	}

	private FrontEndCompressionMiddleware CreateMiddleware()
	{
		var logger = new Mock<ILogger<FrontEndCompressionMiddleware>>();
		_ = logger.Setup(x => x.IsEnabled(LogLevel.Debug)).Returns(false);

		var cache = new Mock<IHybridCache>();
		cache.SetReturnsDefault<Task<(string, byte[])>>(Task.FromResult(("br", _testContent)));

		var hostingEnvironment = new Mock<IUmbrellaWebHostingEnvironment>();
		_ = hostingEnvironment.Setup(x => x.MapPath("~/", true)).Returns(@"C:\Temp");

		var httpHeaderValueUtility = new Mock<IHttpHeaderValueUtility>();
		_ = httpHeaderValueUtility.Setup(x => x.CreateETagHeaderValue(_startJan2019Date, _testContent.LongLength)).Returns("randometag");
		_ = httpHeaderValueUtility.Setup(x => x.CreateLastModifiedHeaderValue(_startJan2019Date)).Returns("lastmodifiedval");

		var mimeTypeUtility = new Mock<IMimeTypeUtility>();
		_ = mimeTypeUtility.Setup(x => x.GetMimeType(".js")).Returns("application/javascript");
		_ = mimeTypeUtility.Setup(x => x.GetMimeType(".css")).Returns("text/css");

		var options = new FrontEndCompressionMiddlewareOptions
		{
			Mappings = new List<FrontEndCompressionMiddlewareMapping>
			{
				new FrontEndCompressionMiddlewareMapping
				{
					AppRelativeFolderPaths = new[] { "/sitefiles" }
				}
			}
		};

		var fileProvider = new Mock<IFileProvider>();

		_ = fileProvider.Setup(x => x.GetFileInfo("/exists.js")).Returns(CreateFileInfo("exists.js", true));
		_ = fileProvider.Setup(x => x.GetFileInfo("/notexists.js")).Returns(CreateFileInfo("notexists.js", false));

		// Create the Middleware
		var middleware = new FrontEndCompressionMiddleware(
			null!,
			logger.Object,
			CoreUtilitiesMocks.CreateCacheKeyUtility(),
			cache.Object,
			hostingEnvironment.Object,
			httpHeaderValueUtility.Object,
			mimeTypeUtility.Object,
			options)
		{
			FileProvider = fileProvider.Object
		};

		return middleware;
	}

	private IFileInfo CreateFileInfo(string fileName, bool exists)
	{
		var file = new Mock<IFileInfo>();
		_ = file.Setup(x => x.Exists).Returns(exists);
		_ = file.Setup(x => x.Name).Returns(fileName);
		_ = file.Setup(x => x.Length).Returns(_testContent.LongLength);
		_ = file.Setup(x => x.LastModified).Returns(_startJan2019Date);
		_ = file.Setup(x => x.CreateReadStream()).Returns(new MemoryStream(_testContent));

		return file.Object;
	}
}