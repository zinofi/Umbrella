using log4net.Core;
using log4net.Util;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbrella.Extensions.Logging.Log4Net.Azure;
using Umbrella.Extensions.Logging.Log4Net.Azure.Configuration;
using Xunit;

namespace Umbrella.Extensions.Logging.Log4Net.Azure.Test
{
    public class AzureTableStorageAppenderTest
    {
        //TODO: When moving to GitHub this connection string needs to be dynamically set somehow before executing the tests
#if DEBUG
        private const string c_StorageConnectionString = "UseDevelopmentStorage=true";
#else
        private const string c_StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=umbrellablobtest;AccountKey=eaxPzjIwVy4WQTCUQnUIL6cIYbzFolVp72nfStCQMNXU8lG4I/zaa2ll1wdiZ2q2h4roIA+DCISXnwhD2nRU0A==;EndpointSuffix=core.windows.net";
#endif

        [Fact]
        public async Task SendBufferTest()
        {
            var appender = CreateAppender();

            List<LoggingEvent> lstEvent = new List<LoggingEvent>();

            for(int i = 1; i <= 20; i++)
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
                Config = new AzureTableStorageLogAppenderOptions
                {
                    AppenderType = AzureTableStorageLogAppenderType.Server,
                    Name = "Umbrella Test Appender",
                    TablePrefix = "UmbrellaServerTest"
                }
            };

            CloudStorageAccount account = CloudStorageAccount.Parse(c_StorageConnectionString);
            appender.TableClient = account.CreateCloudTableClient();

            return appender;
        }
    }
}