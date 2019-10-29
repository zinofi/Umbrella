using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Umbrella.DynamicImage.Abstractions
{
	[StructLayout(LayoutKind.Auto)]
	public readonly struct DynamicImageOptions : IEquatable<DynamicImageOptions>
	{
		#region Public Properties
		public int Width { get; }
		public int Height { get; }
		public DynamicResizeMode ResizeMode { get; }
		public DynamicImageFormat Format { get; }
		/// <summary>
		/// This represents the source path and could be a physical path or URL.
		/// </summary>
		public string SourcePath { get; }
		#endregion

		#region Constructors
		public DynamicImageOptions(string path, int width, int height, DynamicResizeMode resizeMode, DynamicImageFormat format)
		{
			SourcePath = path.ToLowerInvariant();
			Width = width;
			Height = height;
			ResizeMode = resizeMode;
			Format = format;
		}
		#endregion

		#region Overridden Methods
		public override bool Equals(object obj) => obj is DynamicImageOptions options && Equals(options);

		public override int GetHashCode()
		{
			int hashCode = 242587360;
			hashCode = hashCode * -1521134295 + Width.GetHashCode();
			hashCode = hashCode * -1521134295 + Height.GetHashCode();
			hashCode = hashCode * -1521134295 + ResizeMode.GetHashCode();
			hashCode = hashCode * -1521134295 + Format.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SourcePath);

			return hashCode;
		}

		public override string ToString() => $"{SourcePath}-{Width}-{Height}-{ResizeMode}-{Format}";
		#endregion

		#region Operators
		public static explicit operator DynamicImageMapping(DynamicImageOptions options) => new DynamicImageMapping(options.Width, options.Height, options.ResizeMode, options.Format);

		public static bool operator ==(DynamicImageOptions left, DynamicImageOptions right) => left.Equals(right);

		public static bool operator !=(DynamicImageOptions left, DynamicImageOptions right) => !(left == right);
		#endregion

		#region Public Static Methods
		public static bool IsEmpty(DynamicImageOptions options) => options == default;
		#endregion

		#region IEquatable Members
		public bool Equals(DynamicImageOptions other)
			=> Width == other.Width &&
				Height == other.Height &&
				ResizeMode == other.ResizeMode &&
				Format == other.Format &&
				SourcePath == other.SourcePath;
		#endregion
	}
}