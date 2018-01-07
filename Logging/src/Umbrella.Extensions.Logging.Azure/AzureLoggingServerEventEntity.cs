using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Extensions.Logging.Azure
{
    public class AzureLoggingServerEventEntity : TableEntity
    {
        #region Public Properties
        public DateTime EventTimeStamp { get; set; }
        public string ClassName { get; set; }
        public string Exception { get; set; }
        public string FileName { get; set; }
        public string Level { get; set; }
        public string LineNumber { get; set; }
        public string Location { get; set; }
        public string Message { get; set; }
        public string MethodName { get; set; }
        public string Properties { get; set; }
        public string ThreadName { get; set; }
        #endregion

        #region Constructors
        public AzureLoggingServerEventEntity()
        {
        }

        public AzureLoggingServerEventEntity(string level, string message, string exceptionString, string threadName, DateTime timeStamp, LocationInformation locationInfo, Exception exceptionObject, IDictionary properties)
        {
            Level = level;

            //Write additional properties into a single table column
            var sb = new StringBuilder(properties.Count);

            foreach (DictionaryEntry entry in properties)
            {
                sb.AppendLine($"{entry.Key}:{entry.Value}");
            }

            Properties = sb.ToString();
            Message = message + Environment.NewLine + exceptionString;
            ThreadName = threadName;
            EventTimeStamp = timeStamp.ToUniversalTime();
            Location = locationInfo.FullInfo;
            ClassName = locationInfo.ClassName;
            FileName = locationInfo.FileName;
            LineNumber = locationInfo.LineNumber;
            MethodName = locationInfo.MethodName;

            if (exceptionObject != null)
                Exception = exceptionObject.ToString();

            PartitionKey = $"{EventTimeStamp.Hour}-Hours"; ;

            //The row key will be the current date and time in a format that will ensure items are ordered
            //in ascending date order. GUID on the end is to ensure the RowKey is unique where the datetime string clashes with another RowKey.
            RowKey = EventTimeStamp.ToString("yyyy-MM-dd hh:mm:ss.ffffff") + "-" + Guid.NewGuid().ToString();
        } 
        #endregion
    }
}