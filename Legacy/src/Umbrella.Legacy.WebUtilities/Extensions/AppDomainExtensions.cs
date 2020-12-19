using System;
using System.Linq;

namespace Umbrella.Legacy.WebUtilities.Extensions
{
	/// <summary>
	/// Extensions methods for use with the <see cref="AppDomain"/> type.
	/// </summary>
	public static class AppDomainExtensions
	{
		private static readonly bool s_IsOwinApplication;

		/// <summary>
		/// Initializes the <see cref="AppDomainExtensions"/> class.
		/// </summary>
		static AppDomainExtensions()
		{
			s_IsOwinApplication = AppDomain.CurrentDomain.GetAssemblies().Any(x => x.FullName.Contains("Owin"));
		}

		/// <summary>
		/// Determines whether the current web application is an OWIN App.
		/// </summary>
		/// <param name="appDomain">The application domain.</param>
		/// <returns><see langword="true"/> if it is an OWIN App; otherwise <see langword="false"/>.</returns>
		public static bool IsOwinApp(this AppDomain appDomain) => s_IsOwinApplication;
	}
}