using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Umbrella.WebUtilities.Security;

namespace Umbrella.Legacy.WebUtilities.Extensions
{
    public static class IOwinContextExtensions
    {
        public static string GetCurrentRequestNonce(this IOwinContext context) => context.Get<string>(SecurityConstants.DefaultNonceKey);
	}
}
