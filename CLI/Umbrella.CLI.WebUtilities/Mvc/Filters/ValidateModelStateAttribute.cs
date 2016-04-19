using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Umbrella.CLI.WebUtilities.Mvc.Filters
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        #region Private Static Members
        private static readonly JsonSerializerSettings s_JsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        #endregion

        #region Overridden Methods
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var modelState = context.ModelState;

            if (!modelState.IsValid)
                context.Result = new JsonResult(modelState, s_JsonSerializerSettings) { StatusCode = 400 };
        }
        #endregion
    }
}