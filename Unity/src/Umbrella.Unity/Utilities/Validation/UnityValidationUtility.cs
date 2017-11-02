using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Umbrella.Unity.Utilities.Validation
{
    public class UnityValidationUtility : IUnityValidationUtility
    {
        public struct UnityValidationResult
        {
            public UnityValidationResult(bool isValid, List<ValidationResult> results) : this()
            {
                IsValid = isValid;
                Results = results;
            }

            public bool IsValid { get; set; }
            public List<ValidationResult> Results { get; set; }
        }

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
        public UnityValidationResult TryValidateModel<TModel>(TModel model)
        {
            try
            {
                var lstValidationResult = new List<ValidationResult>();

                var ctx = new ValidationContext(model);
                bool isValid = Validator.TryValidateObject(model, ctx, lstValidationResult, true);

                return new UnityValidationResult(isValid, lstValidationResult);
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
                var result = TryValidateModel(model);

                if (m_Logger.IsEnabled(LogLevel.Debug))
                    result.Results.ForEach(x => m_Logger.LogDebug(x.ErrorMessage));

                if (!result.IsValid)
                    throw new UnityValidationException(result.Results[0].ErrorMessage);
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