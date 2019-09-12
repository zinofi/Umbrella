using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelState
{
	// TODO: Do we still need this stuff now we have the ProblemDetails stuff in ASP.NET Core 2.1??
    public interface IModelStateTransformer
    {
        object TransformToObject(ModelStateDictionary modelState);
    }

    public interface IModelStateTransformer<T> : IModelStateTransformer
    {
        T Transform(ModelStateDictionary modelState);
    }
}
