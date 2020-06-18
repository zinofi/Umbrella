using System;
using System.Collections;
using System.Text;
using Microsoft.Azure.Cosmos.Table;

namespace Umbrella.Extensions.Logging.Azure
{
	/// <summary>
	/// A table entity used to log client events to Azure Table Storage.
	/// </summary>
	/// <seealso cref="Microsoft.Azure.Cosmos.Table.TableEntity" />
	public class AzureLoggingClientEventEntity : TableEntity
	{
		#region Public Properties		
		/// <summary>
		/// Gets or sets the event time stamp.
		/// </summary>
		public DateTime EventTimeStamp { get; set; }

		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the level.
		/// </summary>
		public string Level { get; set; }

		/// <summary>
		/// Gets or sets the properties.
		/// </summary>
		public string Properties { get; set; }
		#endregion

		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="AzureLoggingClientEventEntity"/> class.
		/// </summary>
		public AzureLoggingClientEventEntity()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AzureLoggingClientEventEntity"/> class.
		/// </summary>
		/// <param name="level">The level.</param>
		/// <param name="message">The message.</param>
		/// <param name="exceptionString">The exception string.</param>
		/// <param name="timeStamp">The time stamp.</param>
		/// <param name="properties">The properties.</param>
		public AzureLoggingClientEventEntity(string level, string message, string exceptionString, DateTime timeStamp, IDictionary properties)
		{
			Level = level;
			Message = message + Environment.NewLine + exceptionString;
			EventTimeStamp = timeStamp.ToUniversalTime();

			//Write additional properties into a single table column
			var sb = new StringBuilder(properties.Count);

			foreach (DictionaryEntry entry in properties)
			{
				sb.AppendLine($"{entry.Key}:{entry.Value}");
			}

			Properties = sb.ToString();

			PartitionKey = $"{EventTimeStamp.Hour}-Hours"; ;

			//The row key will be the current date and time in a format that will ensure items are ordered
			//in ascending date order. GUID on the end is to ensure the RowKey is unique where the datetime string clashes with another RowKey.
			RowKey = EventTimeStamp.ToString("yyyy-MM-dd hh:mm:ss.ffffff") + "-" + Guid.NewGuid().ToString();
		}
		#endregion
	}
}