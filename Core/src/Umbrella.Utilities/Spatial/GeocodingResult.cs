using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Spatial
{
    public class GeocodingResult
    {
		public GeoLocation Location { get; set; }
		public string Postcode { get; set; } = "";
		public string Locality { get; set; } = "";
		public string WiderLocality { get; set; } = "";
		public string Country { get; set; } = "";

	}
}