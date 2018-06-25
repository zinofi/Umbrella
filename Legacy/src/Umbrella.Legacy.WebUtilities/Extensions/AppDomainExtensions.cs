using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Legacy.WebUtilities.Extensions
{
	public static class AppDomainExtensions
	{
		private static readonly bool s_IsOwinApplication;

		static AppDomainExtensions()
		{
			s_IsOwinApplication = AppDomain.CurrentDomain.GetAssemblies().Any(x => x.FullName.Contains("Owin"));
		}

		public static bool IsOwinApp(this AppDomain appDomain)
		{
			return s_IsOwinApplication;
		}
	}
}