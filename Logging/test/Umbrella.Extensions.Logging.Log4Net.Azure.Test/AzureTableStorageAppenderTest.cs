// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using log4net.Core;
using log4net.Util;
using Microsoft.Azure.Cosmos.Table;
using Umbrella.Extensions.Logging.Azure.Configuration;
using Xunit;

namespace Umbrella.Extensions.Logging.Log4Net.Azure.Test;

public class AzureTableStorageAppenderTest
{
#if AZUREDEVOPS
	private static readonly string _storageConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString")!;
#else
	private const string _storageConnectionString = "UseDevelopmentStorage=true";
#endif

	[Fact]
	[Obsolete]
	public async Task SendBufferTest()
	{
		var appender = CreateAppender();

		var lstEvent = new List<LoggingEvent>();

		for (int i = 1; i <= 20; i++)
		{
			lstEvent.Add(new LoggingEvent(new LoggingEventData
			{
				Level = Level.Info,
				Properties = new PropertiesDictionary
				{
					["TestPropertyStringValue"] = $"TestPropertyStringValue-{i}",
					["TestPropertyInt32Value"] = i
				},
				Message = $"This is a test message to log-{i}",
				ThreadName = $"TestThreadName-{i}",
				TimeStampUtc = DateTime.UtcNow,
				LocationInfo = new LocationInfo($"TestClass-{i}", $"TestMethod-{i}", $@"C:\TestFolder\TestClass{1}.cs", i.ToString()),
				ExceptionString = $"This is a test exception string {i}"
			}));
		}

		await appender.SendBufferInternal(lstEvent.ToArray());
	}

	private AzureTableStorageAppender CreateAppender()
	{
		var appender = new AzureTableStorageAppender
		{
			Config = new AzureTableStorageLogAppenderOptions("Umbrella Test Appender", "UmbrellaServerTest", AzureTableStorageLogAppenderType.Server)
		};

		var account = CloudStorageAccount.Parse(_storageConnectionString);
		appender.TableClient = account.CreateCloudTableClient();

		return appender;
	}
}