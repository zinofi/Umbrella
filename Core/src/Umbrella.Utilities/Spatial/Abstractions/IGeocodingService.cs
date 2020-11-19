using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Spatial.Abstractions
{
	public interface IGeocodingService
	{
		Task<(bool success, GeocodingResult result)> GetGeocodingDataItemByPostcodeAsync(string postcode, CancellationToken cancellationToken = default);
		Task<(bool success, IReadOnlyCollection<GeocodingResult> results)> GetGeocodingDataItemsByPostcodesAsync(IEnumerable<string> postcodes, CancellationToken cancellationToken = default);
		Task<(bool success, GeocodingResult result)> GetGeocodingDataItemByGeoLocationAsync(GeoLocation geoLocation, CancellationToken cancellationToken = default);
		Task<(bool success, IReadOnlyCollection<GeocodingResult> results)> GetGeocodingDataItemsByGeoLocationsAsync(IEnumerable<GeoLocation> geoLocations, CancellationToken cancellationToken = default);
	}
}