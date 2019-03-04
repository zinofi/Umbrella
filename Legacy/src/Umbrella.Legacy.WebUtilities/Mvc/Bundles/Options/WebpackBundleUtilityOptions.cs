using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Legacy.WebUtilities.Mvc.Bundles.Options
{
    public class WebpackBundleUtilityOptions : BundleUtilityOptions
    {
		public string ManifestJsonFileName { get; set; } = "manifest";
	}
}