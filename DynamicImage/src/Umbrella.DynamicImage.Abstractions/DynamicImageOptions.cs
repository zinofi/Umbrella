using System.Runtime.InteropServices;

namespace Umbrella.DynamicImage.Abstractions;

/// <summary>
/// Represents the options for resizing a single image.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly record struct DynamicImageOptions
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

	/// <summary>
	/// This represents the source path and could be a physical path or URL.
	/// </summary>
	public string SourcePath { get; }
	#endregion

	#region Constructors		
	/// <summary>
	/// Initializes a new instance of the <see cref="DynamicImageOptions"/> struct.
	/// </summary>
	/// <param name="path">The path.</param>
	/// <param name="width">The width.</param>
	/// <param name="height">The height.</param>
	/// <param name="resizeMode">The resize mode.</param>
	/// <param name="format">The format.</param>
	public DynamicImageOptions(
		string path,
		int width,
		int height,
		DynamicResizeMode resizeMode,
		DynamicImageFormat format)
	{
		SourcePath = path;
		Width = width;
		Height = height;
		ResizeMode = resizeMode;
		Format = format;
	}
	#endregion

	#region Operators
	/// <summary>
	/// Performs an explicit conversion from <see cref="DynamicImageOptions"/> to <see cref="DynamicImageMapping"/>.
	/// </summary>
	/// <param name="options">The options.</param>
	/// <returns>
	/// The result of the conversion.
	/// </returns>
	public static explicit operator DynamicImageMapping(in DynamicImageOptions options) => new(options.Width, options.Height, options.ResizeMode, options.Format);

	/// <summary>
	/// Implements the operator ==.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	/// The result of the operator.
	/// </returns>
	public static bool operator ==(in DynamicImageOptions left, in DynamicImageOptions right) => left.Equals(right);

	/// <summary>
	/// Implements the operator !=.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	/// The result of the operator.
	/// </returns>
	public static bool operator !=(in DynamicImageOptions left, in DynamicImageOptions right) => !(left == right);
	#endregion

	#region Public Static Methods		
	/// <summary>
	/// Determines whether the specified options is empty.
	/// </summary>
	/// <param name="options">The options.</param>
	/// <returns>
	///   <c>true</c> if the specified options is empty; otherwise, <c>false</c>.
	/// </returns>
	public static bool IsEmpty(in DynamicImageOptions options) => options == default;
	#endregion
}