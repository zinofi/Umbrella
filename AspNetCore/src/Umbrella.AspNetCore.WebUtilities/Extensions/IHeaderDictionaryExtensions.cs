using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Umbrella.AppFramework.Shared.Constants;
using Umbrella.AppFramework.Shared.Enumerations;
using Umbrella.AppFramework.Shared.Primitives;

namespace Umbrella.AspNetCore.WebUtilities.Extensions;

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

	/// <summary>
	/// Gets a collection of language codes from the Accept-Language header ordered by quality.
	/// </summary>
	/// <param name="headers">The headers.</param>
	/// <returns>A collection of language codes ordered by quality.</returns>
	public static IReadOnlyCollection<string> GetOrderedAcceptLanguages(this IHeaderDictionary headers)
	{
		string? acceptLanguage = headers?.AcceptLanguage.FirstOrDefault();

		if (string.IsNullOrEmpty(acceptLanguage))
			return [];

		return acceptLanguage.Split(',')
			.Select(x => StringWithQualityHeaderValue.Parse(x))
			.OrderByDescending(lang => lang.Quality ?? 1.0)
			.Select(lang => lang.Value.ToString())
			.Where(lang => !string.IsNullOrWhiteSpace(lang))
			.ToArray();
	}
}