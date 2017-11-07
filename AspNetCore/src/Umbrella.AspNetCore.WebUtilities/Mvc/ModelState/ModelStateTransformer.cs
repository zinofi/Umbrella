using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly ModelStateTransformerOptions m_TransformerOptions;
        private readonly IMemoryCache m_MemoryCache;

        public ModelStateTransformer(ModelStateTransformerOptions transformerOptions,
            IMemoryCache memoryCache)
        {
            m_TransformerOptions = transformerOptions;
            m_MemoryCache = memoryCache;
        }

        public object TransformToObject(ModelStateDictionary modelState)
        {
            TModelState state = new TModelState();

            string TransformKey(string key) => m_TransformerOptions.UseCamelCaseCaseForKeys ? key.ToCamelCaseInvariant() : key;

            foreach (var item in modelState)
            {
                if (item.Value.ValidationState == ModelValidationState.Invalid)
                {
                    string key = m_MemoryCache.GetOrCreate(item.Key, entry =>
                    {
                        entry.SetSlidingExpiration(TimeSpan.FromHours(1)).SetPriority(CacheItemPriority.Low);

                        return !string.IsNullOrWhiteSpace(item.Key) ? string.Join(".", item.Key.Split('.').Select(x => TransformKey(x))) : TransformKey(item.Key);
                    });

                    state.Entries.Add(new TModelStateEntry
                    {
                        Key = key,
                        Errors = item.Value.Errors.Select(x => x.ErrorMessage).ToList()
                    });
                }
            }

            return state;
        }

        public TModelState Transform(ModelStateDictionary modelState) => (TModelState)TransformToObject(modelState);
    }
}