﻿using System.Runtime.InteropServices;

namespace Umbrella.DynamicImage.Abstractions;

// TODO: This will need to be updated to include quality, focal point, etc.

/// <summary>
/// Used to specify the details of a valid Dynamic Image mapping. One or more of these mapping are used to restrict what Dynamic Images can be generated.
/// This is primarily a mechanism to prevent user tampering when parsing image URLs to ensure only image sizes the target application needs are generated.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly record struct DynamicImageMapping
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
}