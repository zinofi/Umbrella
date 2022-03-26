using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Umbrella.AppFramework.Shared.Constants;
using Umbrella.AppFramework.Shared.Enumerations;
using Umbrella.AppFramework.Shared.Primitives;

namespace Umbrella.AspNetCore.WebUtilities.Extensions
{
	/// <summary>
	/// Contains extension methods for use with the <see cref="IHeaderDictionary"/> type.
	/// </summary>
	public static class IHeaderDictionaryExtensions
	{
		/// <summary>
		/// Gets the application client information from the HTTP Headers.
		/// </summary>
		/// <param name="headers">The headers.</param>
		/// <returns>The app client information if all necessary data has been specified in the headers; otherwise null.</returns>
		/// <remarks>
		/// This method will only return an instance of <see cref="AppClientInfo" /> if header values can be found for
		/// <see cref="AppHttpHeaderName.AppClientId"/>, <see cref="AppHttpHeaderName.AppClientType"/> and <see cref="AppHttpHeaderName.AppClientVersion"/>
		/// and these values can be parsed successfully.
		/// </remarks>
		public static AppClientInfo? GetAppClientInfo(this IHeaderDictionary headers)
		{
			string? strClientId = headers.GetCommaSeparatedValues(AppHttpHeaderName.AppClientId).SingleOrDefault();
			string? strClientType = headers.GetCommaSeparatedValues(AppHttpHeaderName.AppClientType).SingleOrDefault();
			string? strClientVersion = headers.GetCommaSeparatedValues(AppHttpHeaderName.AppClientVersion).SingleOrDefault();

			if (string.IsNullOrWhiteSpace(strClientId) || string.IsNullOrWhiteSpace(strClientType) || string.IsNullOrWhiteSpace(strClientVersion))
				return null;

			if (!Enum.TryParse(strClientType, out AppClientType clientType))
				return null;

			if (!Version.TryParse(strClientVersion, out Version? version))
				return null;

			return new AppClientInfo(strClientId, clientType, version);
		}
	}
}