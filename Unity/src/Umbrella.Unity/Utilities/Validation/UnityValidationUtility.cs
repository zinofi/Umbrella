using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Umbrella.Unity.Utilities.Validation
{
    public class UnityValidationUtility : IUnityValidationUtility
    {
        #region Private Members
        private readonly ILogger m_Logger;
        #endregion

        #region Constructors
        public UnityValidationUtility(ILoggerFactory loggerFactory)
        {
            m_Logger = loggerFactory.CreateLogger<UnityValidationUtility>();
        }
        #endregion

        #region IUnityValidationUtility Members
        public (bool IsValid, List<ValidationResult> Results) TryValidateModel<TModel>(TModel model)
        {
            try
            {
                var lstValidationResult = new List<ValidationResult>();

                var ctx = new ValidationContext(model);
                bool isValid = Validator.TryValidateObject(model, ctx, lstValidationResult, true);

                return (isValid, lstValidationResult);
            }
            catch (Exception exc)
            {
                m_Logger.WriteError(exc);
                throw;
            }
        }

        public void ValidateModel<TModel>(TModel model)
        {
            try
            {
                var (isValid, results) = TryValidateModel(model);

                if (m_Logger.IsEnabled(LogLevel.Debug))
                    results.ForEach(x => m_Logger.LogDebug(x.ErrorMessage));

                if (!isValid)
                    throw new UnityValidationException(results[0].ErrorMessage);
            }
            catch (Exception exc)
            {
                if(exc is UnityValidationException == false)
                    m_Logger.WriteError(exc);

                throw;
            }
        } 
        #endregion
    }
}