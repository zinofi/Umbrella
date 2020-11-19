using System;

namespace Umbrella.Utilities.Spatial
{
	// TODO PR: Make readonly when JsonConstructor becomes a thing in .NET 5.
	public struct GeoLocation : IEquatable<GeoLocation>
	{
		public GeoLocation(double latitude, double longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
		}

		public double Latitude { get; set; }
		public double Longitude { get; set; }

		/// <inheritdoc />
		public override readonly bool Equals(object obj) => obj is GeoLocation location && Equals(location);

		public readonly bool Equals(GeoLocation other) => Latitude == other.Latitude &&
				   Longitude == other.Longitude;

		/// <inheritdoc />
		public override readonly int GetHashCode()
		{
			int hashCode = -1416534245;
			hashCode = hashCode * -1521134295 + Latitude.GetHashCode();
			hashCode = hashCode * -1521134295 + Longitude.GetHashCode();
			return hashCode;
		}

		public readonly double DistanceTo(in GeoLocation other, in GeoLocationDistanceType distanceType = GeoLocationDistanceType.Meters)
		{
			const double piDeg180 = Math.PI / 180;

			double d1 = Latitude * piDeg180;
			double num1 = Longitude * piDeg180;
			double d2 = other.Latitude * piDeg180;
			double num2 = other.Longitude * piDeg180 - num1;
			double d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

			double distanceMeters = 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));

			return distanceType switch
			{
				GeoLocationDistanceType.Meters => distanceMeters,
				GeoLocationDistanceType.Kilometers => distanceMeters / 1000,
				GeoLocationDistanceType.Miles => distanceMeters * 0.000621371,
				GeoLocationDistanceType.NauticalMiles => distanceMeters * 0.000539957,
				_ => distanceMeters
			};
		}

		public static bool operator ==(in GeoLocation left, in GeoLocation right) => left.Equals(right);

		public static bool operator !=(in GeoLocation left, in GeoLocation right) => !(left == right);
	}
}