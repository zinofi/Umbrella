using System;
using Microsoft.Owin;

namespace Umbrella.Kentico.Utilities.ContactManagement.Abstractions
{
	public interface IKenticoContactManager
    {
		void Merge(string userName);
		void ContingentMerge(IOwinContext owinContext, string currentSiteName, bool reset, Func<CookieOptions> cookieOptionsBuilder);
	}
}