using System;
using System.Runtime.InteropServices;

namespace Umbrella.DynamicImage.Abstractions;

/// <summary>
/// Used to specify the details of a valid Dynamic Image mapping. One or more of these mapping are used to restrict what Dynamic Images can be generated.
/// This is primarily a mechanism to prevent user tampering when parsing image URLs to ensure only image sizes the target application needs are generated.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct DynamicImageMapping : IEquatable<DynamicImageMapping>
{
	#region Public Properties		
	/// <summary>
	/// Gets the width.
	/// </summary>
	public int Width { get; }

	/// <summary>
	/// Gets the height.
	/// </summary>
	public int Height { get; }

	/// <summary>
	/// Gets the resize mode.
	/// </summary>
	public DynamicResizeMode ResizeMode { get; }

	/// <summary>
	/// Gets the format.
	/// </summary>
	public DynamicImageFormat Format { get; }
	#endregion

	#region Constructors		
	/// <summary>
	/// Initializes a new instance of the <see cref="DynamicImageMapping"/> struct.
	/// </summary>
	/// <param name="width">The width.</param>
	/// <param name="height">The height.</param>
	/// <param name="resizeMode">The resize mode.</param>
	/// <param name="format">The format.</param>
	public DynamicImageMapping(
		int width,
		int height,
		DynamicResizeMode resizeMode,
		DynamicImageFormat format)
	{
		Width = width;
		Height = height;
		ResizeMode = resizeMode;
		Format = format;
	}
	#endregion

	#region Overridden Methods
	/// <inheritdoc />
	public override string ToString() => $"{Width}-{Height}-{ResizeMode}-{Format}";

	/// <inheritdoc />
	public override bool Equals(object obj) => obj is DynamicImageMapping mapping && Equals(mapping);

	/// <inheritdoc />
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
	/// <inheritdoc />
	public bool Equals(DynamicImageMapping other)
		=> Width == other.Width &&
			Height == other.Height &&
			ResizeMode == other.ResizeMode &&
			Format == other.Format;
	#endregion

	#region Operators		
	/// <summary>
	/// Implements the operator ==.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	/// The result of the operator.
	/// </returns>
	public static bool operator ==(DynamicImageMapping left, DynamicImageMapping right) => left.Equals(right);

	/// <summary>
	/// Implements the operator !=.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	/// The result of the operator.
	/// </returns>
	public static bool operator !=(DynamicImageMapping left, DynamicImageMapping right) => !(left == right);
	#endregion
}