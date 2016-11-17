using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;
using Umbrella.WebUtilities.ModelState;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelState
{
    public class ModelStateTransformer<TModelState, TModelStateEntry> : IModelStateTransformer<TModelState>, IModelStateTransformer
        where TModelState : DefaultTransformedModelState<TModelStateEntry>, new()
        where TModelStateEntry : DefaultTransformedModelStateEntry, new()
    {
        public object TransformToObject(ModelStateDictionary modelState)
        {
            TModelState state = new TModelState();

            foreach (var item in modelState)
            {
                if (item.Value.ValidationState == ModelValidationState.Invalid)
                {
                    state.Entries.Add(new TModelStateEntry
                    {
                        //TODO: Cache the transformed keys.
                        //TODO: inject something into this type to determine whether the key is emitted in CamelCase or PascalCase instead of forcing to be CamelCase.
                        Key = !string.IsNullOrWhiteSpace(item.Key) ? string.Join(".", item.Key.Split('.').Select(x => x.ToCamelCase())) : item.Key.ToCamelCase(),
                        Errors = item.Value.Errors.Select(x => x.ErrorMessage).ToList()
                    });
                }
            }

            return state;
        }

        public TModelState Transform(ModelStateDictionary modelState)
        {
            return (TModelState)TransformToObject(modelState);
        }
    }
}