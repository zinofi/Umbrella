using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Extensions
{
    public static class ILoggerExtensions
    {
        #region Public Static Methods
        public static bool LogError(this ILogger log, Exception exc, object model = null, string message = null, bool returnValue = false, [CallerMemberName]string methodName = "")
        {
            LogErrorDetails(log, exc, model, message, methodName);

            AggregateException aggregateException = exc as AggregateException;

            if (aggregateException?.InnerExceptions != null && aggregateException.InnerExceptions.Count > 0)
            {
                foreach (Exception excInner in aggregateException.InnerExceptions)
                {
                    LogErrorDetails(log, excInner, model, message, methodName);
                }
            }

            return returnValue;
        }
        #endregion

        #region Private Static Methods
        private static void LogErrorDetails(ILogger log, Exception exc, object model, string message, string methodName)
        {
            StringBuilder messageBuilder = new StringBuilder();

            if (model != null)
            {
                string jsonModel = JsonConvert.SerializeObject(model);
                messageBuilder.AppendFormat("{0}({1}) failed", methodName, jsonModel);
            }
            else
            {
                messageBuilder.AppendFormat("{0}() failed", methodName);
            }

            if (!string.IsNullOrEmpty(message))
                messageBuilder.Append(" - " + message);

            log.LogError(messageBuilder.ToString(), exc);
        }
        #endregion
    }
}