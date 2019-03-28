using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Umbrella.Kentico.Utilities.Middleware.Options
{
    public class MergeMarketingContactMiddlewareOptions
    {
		public string KenticoSiteName { get; set; }
		public string IsSigningInClaimType { get; set; }
		public bool CookieSecure { get; set; }
		public string CookieDomain { get; set; }
		public TimeSpan CookieExpiration { get; set; }
		public bool LogLoginActivity { get; set; }
		public Func<string, bool> ShouldExecuteForPathDeterminer { get; set; }
	}
}