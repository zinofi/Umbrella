using log4net;
using log4net.Appender;
using log4net.Core;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Extensions.Logging.Log4Net.Azure.Configuration;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;

namespace Umbrella.Extensions.Logging.Log4Net.Azure
{
    public class AzureTableStorageAppender : BufferingAppenderSkeleton
    {
        #region Public Constants
        public const string TableNameSeparator = "xxxxxx";
        #endregion

        #region Private Members
        private AzureTableStorageLogAppenderOptions m_Config;
        private CloudStorageAccount m_Account;
        private CloudTableClient m_Client;
        private bool m_LogErrorsToConsole;
        #endregion

        #region Overridden Methods
        protected override void SendBuffer(LoggingEvent[] events)
        {
            //Executing this code asynchronously on a worker thread to avoid blocking the main thread
            //If there are a lot of threads trying to write to the logs then this could have been a bottleneck
            //if executed synchronously.
            Task logTask = Task.Run(async () =>
            {
                if (m_LogErrorsToConsole)
                    Console.WriteLine("SendBuffer started.");

                try
                {
                    if (m_Config == null)
                        throw new Exception($"The log4net {nameof(AzureTableStorageAppender)} with name: {Name} has not been initialized. The {nameof(InitializeAppender)} must be called from your code before the log appender is first used.");

                    //Get the table we need to write stuff to and create it if needed
                    CloudTable table = m_Client.GetTableReference($"{m_Config.TablePrefix}{TableNameSeparator}{DateTime.UtcNow.ToString("yyyyxMMxdd")}");
                    await table.CreateIfNotExistsAsync().ConfigureAwait(false);

                    //Create the required table entities to write to storage and group them by PartitionKey.
                    //This is because entities written in a batch must all have the same PartitionKey.
                    var paritionKeyGroups = events.Select(x => GetLogEntity(x)).GroupBy(x => x.PartitionKey);

                    foreach (var group in paritionKeyGroups)
                    {
                        foreach (var batch in group.Split(100))
                        {
                            var batchOperation = new TableBatchOperation();

                            foreach (var item in batch)
                            {
                                if (item != null)
                                    batchOperation.Insert(item);
                            }

                            await table.ExecuteBatchAsync(batchOperation).ConfigureAwait(false);
                        }
                    }
                }
                catch (StorageException exc)
                {
                    if (m_LogErrorsToConsole)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(exc.Message);
                        Console.WriteLine($"HttpStatusCode: {exc.RequestInformation.HttpStatusCode}, ErrorCode: {exc.RequestInformation.ExtendedErrorInformation.ErrorCode}, ErrorMessage: {exc.RequestInformation.ExtendedErrorInformation.ErrorMessage}");
                        Console.WriteLine($"AdditionalDetails: {exc.RequestInformation.ExtendedErrorInformation.AdditionalDetails.ToJsonString()}");
                        Console.ResetColor();
                    }

                    throw;
                }
            });

            logTask.GetAwaiter().OnCompleted(() =>
            {
                if (logTask.IsFaulted)
                    throw logTask.Exception;
            });
        }
        #endregion

        #region Public Methods
        public void InitializeAppender(AzureTableStorageLogAppenderOptions options, string connectionString, bool logErrorsToConsole)
        {
            Guard.ArgumentNotNull(options, nameof(options));
            Guard.ArgumentNotNullOrWhiteSpace(connectionString, nameof(connectionString));
            Guard.ArgumentNotNullOrWhiteSpace(options.Name, nameof(options.Name));
            Guard.ArgumentNotNullOrWhiteSpace(options.TablePrefix, nameof(options.TablePrefix));

            m_Config = options;

            //Get both the account and create the table client here.
            //We will get a reference to the table when we need to write to it as the name of the table needs to change
            //to reflect the current date.
            m_Account = CloudStorageAccount.Parse(connectionString);
            m_Client = m_Account.CreateCloudTableClient();

            m_LogErrorsToConsole = logErrorsToConsole;

            ActivateOptions();
        }

        public static void InitializeAllAppenders(AzureTableStorageLoggingOptions options, string connectionString)
        {
            Guard.ArgumentNotNull(options, nameof(options));

            foreach (var appender in LogManager.GetAllRepositories().SelectMany(x => x.GetAppenders().OfType<AzureTableStorageAppender>()))
            {
                var config = options.Appenders.SingleOrDefault(x => x.Name == appender.Name);

                if (config == null)
                    throw new Exception($"Configuration cannot be found for appender {appender.Name}");

                appender.InitializeAppender(config, connectionString, options.LogErrorsToConsole);
            }
        }
        #endregion

        #region Private Methods
        private ITableEntity GetLogEntity(LoggingEvent e)
        {
            switch (m_Config.AppenderType)
            {
                case AzureTableStorageLogAppenderType.Client:
                    return new AzureLoggingClientEventEntity(e);
                case AzureTableStorageLogAppenderType.Server:
                    return new AzureLoggingServerEventEntity(e);
            }

            return null;
        } 
        #endregion
    }
}