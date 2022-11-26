using System;
using Umbrella.Utilities.Exceptions;

namespace Umbrella.DynamicImage.Abstractions;

/// <summary>
/// An exception class that represents errors that occur within the Dynamic Image infrastructure.
/// </summary>
/// <seealso cref="UmbrellaException" />
public class DynamicImageException : UmbrellaException
{
	/// <summary>
	/// Gets the options.
	/// </summary>
	public DynamicImageOptions Options { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="DynamicImageException"/> class.
	/// </summary>
	public DynamicImageException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DynamicImageException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="options">The options.</param>
	public DynamicImageException(string message, DynamicImageOptions options = default)
		: base(message)
	{
		Options = options;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DynamicImageException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="innerException">The inner exception.</param>
	/// <param name="options">The options.</param>
	public DynamicImageException(string message, Exception innerException, DynamicImageOptions options = default)
		: base(message, innerException)
	{
		Options = options;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DynamicImageException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="innerException">The inner exception.</param>
	/// <param name="width">The width.</param>
	/// <param name="height">The height.</param>
	/// <param name="resizeMode">The resize mode.</param>
	/// <param name="format">The format.</param>
	public DynamicImageException(string message, Exception innerException, int width, int height, DynamicResizeMode resizeMode, DynamicImageFormat format)
		: base(message, innerException)
	{
		Options = new DynamicImageOptions("", width, height, resizeMode, format);
	}
}