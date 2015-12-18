using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.WebUtilities.DynamicImage.Enumerations;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Configuration
{
	public struct DynamicImageMapping
	{
		public int Width { get; set; }
		public int Height { get; set; }
		public DynamicResizeMode ResizeMode { get; set; }
		public DynamicImageFormat Format { get; set; }

		public override string ToString()
		{
			return string.Format("{0}-{1}-{2}-{3}",
				Width,
				Height,
				ResizeMode,
				Format);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			return Equals((DynamicImageMapping)obj);
		}

		public bool Equals(DynamicImageMapping mapping)
		{
			return this == mapping;
		}

		public static bool operator == (DynamicImageMapping a, DynamicImageMapping b)
		{
			return a.Width == b.Width
				&& a.Height == b.Height
				&& a.ResizeMode == b.ResizeMode
				&& a.Format == b.Format;
		}

		public static bool operator != (DynamicImageMapping a, DynamicImageMapping b)
		{
			return !(a == b);
		}
	}
}