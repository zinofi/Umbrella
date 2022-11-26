// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Text;
using Azure;
using Azure.Data.Tables;

namespace Umbrella.Extensions.Logging.Azure;

/// <summary>
/// A table entity used to log server events to Azure Table Storage.
/// </summary>
/// <seealso cref="ITableEntity" />
public class AzureLoggingServerEventEntity : ITableEntity
{
	#region Public Properties		
	/// <summary>
	/// Gets or sets the event time stamp.
	/// </summary>
	public DateTime EventTimeStamp { get; set; }

	/// <summary>
	/// Gets or sets the name of the class.
	/// </summary>
	public string? ClassName { get; set; }

	/// <summary>
	/// Gets or sets the exception.
	/// </summary>
	public string? Exception { get; set; }

	/// <summary>
	/// Gets or sets the name of the file.
	/// </summary>
	public string? FileName { get; set; }

	/// <summary>
	/// Gets or sets the level.
	/// </summary>
	public string? Level { get; set; }

	/// <summary>
	/// Gets or sets the line number.
	/// </summary>
	public string? LineNumber { get; set; }

	/// <summary>
	/// Gets or sets the location.
	/// </summary>
	public string? Location { get; set; }

	/// <summary>
	/// Gets or sets the message.
	/// </summary>
	public string? Message { get; set; }

	/// <summary>
	/// Gets or sets the name of the method.
	/// </summary>
	public string? MethodName { get; set; }

	/// <summary>
	/// Gets or sets the properties.
	/// </summary>
	public string? Properties { get; set; }

	/// <summary>
	/// Gets or sets the name of the thread.
	/// </summary>
	public string? ThreadName { get; set; }

	/// <inheritdoc />
	public string PartitionKey { get; set; } = null!;

	/// <inheritdoc />
	public string RowKey { get; set; } = null!;

	/// <inheritdoc />
	public DateTimeOffset? Timestamp { get; set; }

	/// <inheritdoc />
	public ETag ETag { get; set; }
	#endregion

	#region Constructors		
	/// <summary>
	/// Initializes a new instance of the <see cref="AzureLoggingServerEventEntity"/> class.
	/// </summary>
	public AzureLoggingServerEventEntity()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AzureLoggingServerEventEntity"/> class.
	/// </summary>
	/// <param name="level">The level.</param>
	/// <param name="message">The message.</param>
	/// <param name="exceptionString">The exception string.</param>
	/// <param name="threadName">Name of the thread.</param>
	/// <param name="timeStamp">The time stamp.</param>
	/// <param name="locationInfo">The location information.</param>
	/// <param name="exceptionObject">The exception object.</param>
	/// <param name="properties">The properties.</param>
	public AzureLoggingServerEventEntity(string level, string message, string exceptionString, string threadName, DateTime timeStamp, LocationInformation locationInfo, Exception exceptionObject, IDictionary properties)
	{
		Level = level;

		//Write additional properties into a single table column
		var sb = new StringBuilder(properties.Count);

		foreach (DictionaryEntry entry in properties)
		{
			_ = sb.AppendLine($"{entry.Key}:{entry.Value}");
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

		if (exceptionObject is not null)
			Exception = exceptionObject.ToString();

		PartitionKey = $"{EventTimeStamp.Hour}-Hours";
		;

		//The row key will be the current date and time in a format that will ensure items are ordered
		//in ascending date order. GUID on the end is to ensure the RowKey is unique where the datetime string clashes with another RowKey.
		RowKey = EventTimeStamp.ToString("yyyy-MM-dd hh:mm:ss.ffffff") + "-" + Guid.NewGuid().ToString();
	}
	#endregion
}