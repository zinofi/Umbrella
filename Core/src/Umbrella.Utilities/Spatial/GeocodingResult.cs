namespace Umbrella.Utilities.Spatial
{
	/// <summary>
	/// Represents the output of a successful geocoding operation.
	/// </summary>
	public class GeocodingResult
    {
		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		public GeoLocation Location { get; set; }

		/// <summary>
		/// Gets or sets the postcode.
		/// </summary>
		public string Postcode { get; set; } = "";

		/// <summary>
		/// Gets or sets the locality.
		/// </summary>
		public string Locality { get; set; } = "";

		/// <summary>
		/// Gets or sets the wider locality.
		/// </summary>
		public string WiderLocality { get; set; } = "";

		/// <summary>
		/// Gets or sets the country.
		/// </summary>
		public string Country { get; set; } = "";

	}
}