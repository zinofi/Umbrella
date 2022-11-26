namespace Umbrella.WebUtilities.SiteMap;

/// <summary>
/// The change frequency for an XML SiteMap element which indicates how often a resource is changed.
/// </summary>
public enum ChangeFrequency
{
	/// <summary>
	/// Specifies that the resource in constantly changing.
	/// </summary>
	Always = 0,

	/// <summary>
	/// Specifies that the resource changes hourly.
	/// </summary>
	Hourly = 1,

	/// <summary>
	/// Specifies that the resource changes daily.
	/// </summary>
	Daily = 2,

	/// <summary>
	/// Specifies that the resource changes weekly.
	/// </summary>
	Weekly = 3,

	/// <summary>
	/// Specifies that the resource changes monthly.
	/// </summary>
	Monthly = 4,

	/// <summary>
	/// Specifies that the resource changes yearly.
	/// </summary>
	Yearly = 5,

	/// <summary>
	/// Specifies that the resource never changes.
	/// </summary>
	Never = 6,
}