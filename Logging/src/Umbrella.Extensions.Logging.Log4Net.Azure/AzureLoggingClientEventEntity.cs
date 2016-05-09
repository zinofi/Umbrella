using log4net.Core;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.Extensions.Logging.Log4Net.Azure
{
    public class AzureLoggingClientEventEntity : TableEntity
    {
        public DateTime EventTimeStamp { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }

        public AzureLoggingClientEventEntity(LoggingEvent e, string partitionKey)
        {
            Level = e.Level.ToString();
            Message = e.RenderedMessage + Environment.NewLine + e.GetExceptionString();
            EventTimeStamp = e.TimeStamp;
            PartitionKey = partitionKey;

            //The row key will be the current date and time in a format that will ensure items are ordered
            //in ascending date order. GUID on the end is to ensure the RowKey is unique where the datetime string clashes with another RowKey.
            RowKey = DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss.ffffff") + "-" + Guid.NewGuid().ToString();
        }
    }
}