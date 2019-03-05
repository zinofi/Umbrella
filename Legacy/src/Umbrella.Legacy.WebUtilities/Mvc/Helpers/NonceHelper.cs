using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Umbrella.Legacy.WebUtilities.Extensions;
using Umbrella.WebUtilities.Exceptions;
using Umbrella.WebUtilities.Security;

namespace Umbrella.Legacy.WebUtilities.Mvc.Helpers
{
    public static class NonceHelper
    {
        public static string GetCurrentRequestNonce(this HtmlHelper htmlHelper)
		{
			string value = htmlHelper.ViewContext.HttpContext.GetOwinContext().GetCurrentRequestNonce();

			if (string.IsNullOrWhiteSpace(value))
				value = htmlHelper.ViewContext.HttpContext.GetCurrentRequestNonce();

			if (string.IsNullOrWhiteSpace(value))
				throw new UmbrellaWebException($"A nonce for the current request could not be found either in the OWIN environment or in the items dictionary of HttpContextBase.");

			return value;
		}
    }
}