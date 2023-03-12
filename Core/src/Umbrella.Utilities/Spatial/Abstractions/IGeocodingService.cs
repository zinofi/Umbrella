namespace Umbrella.Utilities.Spatial.Abstractions;

/// <summary>
/// A service that can be used to geocode one or more postcodes or <see cref="GeoLocation"/> instances and return data about each location.
/// </summary>
public interface IGeocodingService
{
	/// <summary>
	/// Geocodes the specified <paramref name="postcode"/>.
	/// </summary>
	/// <param name="postcode">The postcode.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A tuple which indicates if the operation was a success together with the result.</returns>
	Task<(bool success, GeocodingResult? result)> GetGeocodingDataItemByPostcodeAsync(string postcode, CancellationToken cancellationToken = default);

	/// <summary>
	/// Geocodes the specified <paramref name="postcodes"/>.
	/// </summary>
	/// <param name="postcodes">The postcodes.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A tuple which indicates if the operation was a success together with the results.</returns>
	Task<(bool success, IReadOnlyCollection<GeocodingResult>? results)> GetGeocodingDataItemsByPostcodesAsync(IEnumerable<string> postcodes, CancellationToken cancellationToken = default);

	/// <summary>
	/// Geocodes the specified <paramref name="geoLocation"/>.
	/// </summary>
	/// <param name="geoLocation">The location.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A tuple which indicates if the operation was a success together with the result.</returns>
	Task<(bool success, GeocodingResult? result)> GetGeocodingDataItemByGeoLocationAsync(GeoLocation geoLocation, CancellationToken cancellationToken = default);

	/// <summary>
	/// Geocodes the specified <paramref name="geoLocations"/>.
	/// </summary>
	/// <param name="geoLocations">The locations.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A tuple which indicates if the operation was a success together with the result.</returns>
	Task<(bool success, IReadOnlyCollection<GeocodingResult>? results)> GetGeocodingDataItemsByGeoLocationsAsync(IEnumerable<GeoLocation> geoLocations, CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets the geocoding data for the specified <paramref name="partialPostcode"/>.
	/// </summary>
	/// <param name="partialPostcode">The partial postcode.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>A tuple which indicates if the operation was a success together with the result.</returns>
	Task<(bool success, GeocodingResult? result)> GetGeocodingDataItemByPartialPostcodeAsync(string partialPostcode, CancellationToken cancellationToken = default);
}