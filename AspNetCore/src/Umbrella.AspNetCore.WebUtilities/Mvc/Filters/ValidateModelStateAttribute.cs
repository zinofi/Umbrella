using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.AspNetCore.WebUtilities.Mvc.ModelState;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.Filters
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        #region Private Members
        private readonly IModelStateTransformer m_ModelStateTransformer;
        #endregion

        #region Constructor
        public ValidateModelStateAttribute(IModelStateTransformer modelStateTransformer)
        {
            m_ModelStateTransformer = modelStateTransformer;
        } 
        #endregion

        #region Overridden Methods
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var modelState = context.ModelState;

            if (!modelState.IsValid)
            {
                //Need to transform the ModelState to something that can be used on the client.
                object transformedState = m_ModelStateTransformer.TransformToObject(modelState);

                context.Result = new ObjectResult(transformedState) { StatusCode = 400 };
            }
        }
        #endregion
    }
}