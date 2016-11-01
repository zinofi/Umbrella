using Microsoft.AspNetCore.Mvc.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Umbrella.DataAnnotations;

namespace Umbrella.AspNetCore.DataAnnotations
{
    public class UmbrellaValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        #region Private Members
        private IValidationAttributeAdapterProvider m_MvcFrameworkImplementation = new ValidationAttributeAdapterProvider(); 
        #endregion

        #region IValidationAttributeAdapterProvider Members
        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            IAttributeAdapter adapter = m_MvcFrameworkImplementation.GetAttributeAdapter(attribute, stringLocalizer);

            if(adapter == null)
            {
                var type = attribute.GetType();

                if(type == typeof(UmbrellaPostcodeAttribute) || type == typeof(UmbrellaPhoneAttribute))
                {
                    adapter = new RegularExpressionAttributeAdapter((RegularExpressionAttribute)attribute, stringLocalizer);
                }
                else
                {
                    //TODO: Need to create adapter implementations of all the other attributes in the Umbrella.DataAnnotations library in order
                    //for them to work properly with jQuery unobtrusive validation as they do currently in MVC 5 using the Umbrella.Legacy.WebUtilities.DataAnnotations library.
                }
            }

            return adapter;
        } 
        #endregion
    }
}