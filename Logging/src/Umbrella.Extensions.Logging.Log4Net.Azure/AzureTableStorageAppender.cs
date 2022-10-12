// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Azure.Data.Tables;
using CommunityToolkit.Diagnostics;
using log4net;
using log4net.Appender;
using log4net.Core;
using System.Runtime.CompilerServices;
using System.Text;
using Umbrella.Extensions.Logging.Azure;
using Umbrella.Extensions.Logging.Azure.Configuration;
using Umbrella.Utilities.Extensions;

[assembly: InternalsVisibleTo("Umbrella.Extensions.Logging.Log4Net.Azure.Test")]
namespace Umbrella.Extensions.Logging.Log4Net.Azure;

/// <summary>
/// A custom log appender to write messages to Azure Table Storage.
/// </summary>
/// <seealso cref="BufferingAppenderSkeleton" />
public class AzureTableStorageAppender : BufferingAppenderSkeleton
{
	#region Private Members
	private bool _logErrorsToConsole;
	#endregion

	#region Internal Properties
	//Exposed as internal for unit testing purposes
	internal AzureTableStorageLogAppenderOptions? Config { get; set; }
	internal TableServiceClient? TableClient { get; set; }
	#endregion

	#region Internal Methods
	[Obsolete]
	internal async Task SendBufferInternal(LoggingEvent[] events)
	{
		if (_logErrorsToConsole)
			Console.WriteLine("SendBuffer started.");

		try
		{
			if (Config is null)
				throw new Exception($"The log4net {nameof(AzureTableStorageAppender)} with name: {Name} has not been initialized. The {nameof(InitializeAppender)} must be called from your code before the log appender is first used.");

			////Get the table we need to write stuff to and create it if needed
			//CloudTable table = TableClient!.GetTableReference($"{Config.TablePrefix}{AzureTableStorageLoggingOptions.TableNameSeparator}{DateTime.UtcNow:yyyyxMMxdd}");
			//_ = await table.CreateIfNotExistsAsync().ConfigureAwait(false);

			////Create the required table entities to write to storage and group them by PartitionKey.
			////This is because entities written in a batch must all have the same PartitionKey.
			//var paritionKeyGroups = events.Select(x => GetLogEntity(x)).Where(x => x is not null).GroupBy(x => x!.PartitionKey);

			//foreach (var group in paritionKeyGroups)
			//{
			//	foreach (var batch in group.Split(100))
			//	{
			//		var batchOperation = new TableBatchOperation();

			//		foreach (var item in batch)
			//		{
			//			if (item is not null)
			//				batchOperation.Insert(item);
			//		}

			//		_ = await table.ExecuteBatchAsync(batchOperation).ConfigureAwait(false);
			//	}
			//}
		}
		catch (Exception)
		{
			// TODO: Fix this
			//if (_logErrorsToConsole)
			//{
			//	Console.ForegroundColor = ConsoleColor.Red;
			//	Console.WriteLine(exc.Message);
			//	Console.WriteLine($"HttpStatusCode: {exc.RequestInformation.HttpStatusCode}, ErrorCode: {exc.ErrorCode}, ErrorMessage: {exc.RequestInformation.ExtendedErrorInformation.ErrorMessage}");
			//	Console.WriteLine($"AdditionalDetails: {exc.RequestInformation.ExtendedErrorInformation.AdditionalDetails.ToJsonString()}");
			//	Console.ResetColor();
			//}

			throw;
		}
	}
	#endregion

	#region Overridden Methods
	/// <inheritdoc />
	[Obsolete]
	protected override void SendBuffer(LoggingEvent[] events)
	{
		//Executing this code asynchronously on a worker thread to avoid blocking the main thread
		//If there are a lot of threads trying to write to the logs then this could have been a bottleneck
		//if executed synchronously.
		var logTask = Task.Run(() => SendBufferInternal(events));

		logTask.GetAwaiter().OnCompleted(() =>
		{
			//If there was a problem writing the log message we need some kind of idea of what happened
			//and logging the below information and throwing a new Exception should allow it to be diagnosed from the system
			//event logs.
			if (logTask.IsFaulted)
			{
				//if (logTask.Exception.GetBaseException() is StorageException exc)
				//{
				//	StringBuilder sbMessage = new StringBuilder()
				//		.AppendLine(exc.Message)
				//		.AppendLine($"HttpStatusCode: {exc.RequestInformation.HttpStatusCode}, ErrorCode: {exc.RequestInformation.ErrorCode}, ErrorMessage: {exc.RequestInformation.ExtendedErrorInformation.ErrorMessage}")
				//		.AppendLine($"AdditionalDetails: {exc.RequestInformation.ExtendedErrorInformation.AdditionalDetails.ToJsonString()}");

				//	throw new Exception(sbMessage.ToString(), exc);
				//}
				//else
				//{
				//	throw logTask.Exception;
				//}

				throw logTask.Exception;
			}
		});
	}
	#endregion

	#region Public Methods		
	/// <summary>
	/// Initializes the appender.
	/// </summary>
	/// <param name="options">The options.</param>
	/// <param name="connectionString">The connection string.</param>
	/// <param name="logErrorsToConsole">if set to <c>true</c>, errors that occur during execution of this appender will be logged to the console.</param>
	public void InitializeAppender(AzureTableStorageLogAppenderOptions options, string connectionString, bool logErrorsToConsole)
	{
		Guard.IsNotNull(options, nameof(options));
		Guard.IsNotNullOrWhiteSpace(connectionString, nameof(connectionString));
		Guard.IsNotNullOrWhiteSpace(options.Name, nameof(options.Name));
		Guard.IsNotNullOrWhiteSpace(options.TablePrefix, nameof(options.TablePrefix));

		Config = options;

		//Get both the account and create the table client here.
		//We will get a reference to the table when we need to write to it as the name of the table needs to change
		//to reflect the current date.
		TableClient = new TableServiceClient(connectionString);

		_logErrorsToConsole = logErrorsToConsole;

		ActivateOptions();
	}

	/// <summary>
	/// Initializes all appenders.
	/// </summary>
	/// <param name="options">The options.</param>
	/// <param name="connectionString">The connection string.</param>
	/// <exception cref="Exception">Configuration cannot be found for appender {appender.Name}</exception>
	public static void InitializeAllAppenders(AzureTableStorageLoggingOptions options, string connectionString)
	{
		Guard.IsNotNull(options, nameof(options));

		foreach (var appender in LogManager.GetAllRepositories().SelectMany(x => x.GetAppenders().OfType<AzureTableStorageAppender>()))
		{
			var config = options.Appenders.SingleOrDefault(x => x.Name == appender.Name);

			if (config is null)
				throw new Exception($"Configuration cannot be found for appender {appender.Name}");

			appender.InitializeAppender(config, connectionString, options.LogErrorsToConsole);
		}
	}
	#endregion

	#region Private Methods
	private ITableEntity? GetLogEntity(LoggingEvent e)
	{
		switch (Config!.AppenderType)
		{
			case AzureTableStorageLogAppenderType.Client:
				return new AzureLoggingClientEventEntity(e.Level.ToString(), e.RenderedMessage, e.GetExceptionString(), e.TimeStampUtc, e.Properties);
			case AzureTableStorageLogAppenderType.Server:
				var locationInfo = new LocationInformation(e.LocationInformation.ClassName, e.LocationInformation.FileName, e.LocationInformation.LineNumber, e.LocationInformation.MethodName, e.LocationInformation.FullInfo);
				return new AzureLoggingServerEventEntity(e.Level.ToString(), e.RenderedMessage, e.GetExceptionString(), e.ThreadName, e.TimeStampUtc, locationInfo, e.ExceptionObject, e.Properties);
		}

		return null;
	}
	#endregion
}