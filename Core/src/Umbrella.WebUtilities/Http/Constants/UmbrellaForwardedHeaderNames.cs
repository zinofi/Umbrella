namespace Umbrella.WebUtilities.Http.Constants;

/// <summary>
/// Contains the names of the forwarded headers that are used by Umbrella.
/// </summary>
public static class UmbrellaForwardedHeaderNames
{
	/// <summary>
	/// The name of the header that contains the original host that the request was made to
	/// when the request has been forwarded by Azure Front Door.
	/// </summary>
	/// <remarks>
	/// This is a custom header which must be manually added to the request by Azure Front Door
	/// using the Rule Sets feature. It is not added automatically by Azure Front Door.
	/// </remarks>
	public const string AzureFrontDoorForwardedHost = "X-AFD-Fowarded-Host";
}