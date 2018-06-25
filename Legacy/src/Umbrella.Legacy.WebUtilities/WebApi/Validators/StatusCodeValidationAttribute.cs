using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.WebHost;

namespace Umbrella.Legacy.WebUtilities.WebApi.Validators
{
    /// <summary>
    /// Used to define a custom status code to associate with customly defined validation attributes which will be thrown as a HttpResponseException
    /// when processed through the Web API pipeline. The MVC pipeline will ignore and treat as a standard validation error.
    /// </summary>
    public abstract class StatusCodeValidationAttribute : ValidationAttribute
    {
        private readonly HttpContext m_HttpContext;

        public HttpStatusCode StatusCode { get; set; }

        public StatusCodeValidationAttribute()
        {
        }

        public StatusCodeValidationAttribute(HttpContext context)
        {
            m_HttpContext = context;
        }

        protected ValidationResult HandleInvalidResult(ValidationContext validationContext)
        {
            //Ensure we have the HttpContext - always re-assign the instance variable if not testing
            //because these validation attributes get cached by the framework
            HttpContext context = m_HttpContext ?? HttpContext.Current;

            //Processing via Web API
            if (context.CurrentHandler is HttpControllerHandler)
            {
                ValidationResponse response = new ValidationResponse
                {
                    Message = "The request is invalid.",
                    ModelState = new Dictionary<string, List<string>>
                    {
                        {
                            validationContext.DisplayName,
                            new List<string>
                            {
                                FormatErrorMessage(validationContext.DisplayName)
                            }
                        }
                    }
                };

                HttpResponseMessage message = new HttpResponseMessage { StatusCode = StatusCode, ReasonPhrase = FormatErrorMessage(validationContext.DisplayName) };
                message.Content = new ObjectContent<ValidationResponse>(response, new JsonMediaTypeFormatter());

                throw new HttpResponseException(message);
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
    }

    public class ValidationResponse
    {
        public string Message { get; set; }
        public Dictionary<string, List<string>> ModelState { get; set; }
    }
}
