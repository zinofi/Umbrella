using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Helpers
{
    public static class PathHelper
    {
		public static string PlatformNormalize(string path) => string.IsNullOrWhiteSpace(path) ? path : path.Replace('\\', Path.DirectorySeparatorChar);
	}
}