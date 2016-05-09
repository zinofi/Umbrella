using Microsoft.AspNet.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelState
{
    public interface IModelStateTransformer
    {
        object TransformToObject(ModelStateDictionary modelState);
    }

    public interface IModelStateTransformer<T> : IModelStateTransformer
    {
        T Transform(ModelStateDictionary modelState);
    }
}
