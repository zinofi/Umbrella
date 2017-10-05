using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Umbrella.Unity.Utilities.Validation
{
    public interface IUnityValidationUtility
    {
        (bool IsValid, List<ValidationResult> Results) TryValidateModel<TModel>(TModel model);
        void ValidateModel<TModel>(TModel model);
    }
}