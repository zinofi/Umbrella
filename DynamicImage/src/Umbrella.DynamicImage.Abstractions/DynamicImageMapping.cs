using System;
using System.Runtime.InteropServices;

namespace Umbrella.DynamicImage.Abstractions
{
	[StructLayout(LayoutKind.Auto)]
	public readonly struct DynamicImageMapping : IEquatable<DynamicImageMapping>
	{
		#region Public Properties
		public int Width { get; }
		public int Height { get; }
		public DynamicResizeMode ResizeMode { get; }
		public DynamicImageFormat Format { get; }
		#endregion

		#region Constructors
		public DynamicImageMapping(int width, int height, DynamicResizeMode resizeMode, DynamicImageFormat format)
		{
			Width = width;
			Height = height;
			ResizeMode = resizeMode;
			Format = format;
		}
		#endregion

		#region Overridden Methods
		public override string ToString() => $"{Width}-{Height}-{ResizeMode}-{Format}";

		public override bool Equals(object obj) => obj is DynamicImageMapping mapping && Equals(mapping);

		public override int GetHashCode()
		{
			int hashCode = -1481175575;
			hashCode = hashCode * -1521134295 + Width.GetHashCode();
			hashCode = hashCode * -1521134295 + Height.GetHashCode();
			hashCode = hashCode * -1521134295 + ResizeMode.GetHashCode();
			hashCode = hashCode * -1521134295 + Format.GetHashCode();
			return hashCode;
		}
		#endregion

		#region IEquatable Members
		public bool Equals(DynamicImageMapping other)
			=> Width == other.Width &&
				Height == other.Height &&
				ResizeMode == other.ResizeMode &&
				Format == other.Format;
		#endregion

		#region Operators
		public static bool operator ==(DynamicImageMapping left, DynamicImageMapping right) => left.Equals(right);

		public static bool operator !=(DynamicImageMapping left, DynamicImageMapping right) => !(left == right);
		#endregion
	}
}