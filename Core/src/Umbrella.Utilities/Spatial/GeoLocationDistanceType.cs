namespace Umbrella.Utilities.Spatial;

/// <summary>
/// The type of units used for distances between <see cref="GeoLocation"/> instances.
/// </summary>
public enum GeoLocationDistanceType
{
	/// <summary>
	/// Meters
	/// </summary>
	Meters,

	/// <summary>
	/// Kilometers
	/// </summary>
	Kilometers,

	/// <summary>
	/// Miles
	/// </summary>
	Miles,

	/// <summary>
	/// Nautical miles
	/// </summary>
	NauticalMiles
}