using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Umbrella.Unity.Utilities.Validation.UnityValidationUtility;

namespace Umbrella.Unity.Utilities.Validation
{
    public interface IUnityValidationUtility
    {
        UnityValidationResult TryValidateModel<TModel>(TModel model);
        void ValidateModel<TModel>(TModel model);
    }
}