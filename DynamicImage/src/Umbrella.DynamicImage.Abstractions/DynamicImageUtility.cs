// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Constants;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DynamicImage.Abstractions;

/// <summary>
/// Contains utility methods for common operations performed by the Dynamic Image infrastructure.
/// </summary>
/// <seealso cref="IDynamicImageUtility" />
public class DynamicImageUtility : IDynamicImageUtility
{
	#region Private Constants
	private const string VirtualPathFormat = "~/{0}/{1}/{2}/{3}/{4}/{5}";
	#endregion

	#region Private Static Members
	private static readonly (DynamicImageParseUrlResult, DynamicImageOptions) _invalidParseUrlResult = (DynamicImageParseUrlResult.Invalid, default);
	private static readonly (DynamicImageParseUrlResult, DynamicImageOptions) _skipParseUrlResult = (DynamicImageParseUrlResult.Skip, default);
	private static readonly Regex _densityRegex = new("@([0-9]*)x$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	private static readonly char[] _segmentSeparatorArray = new[] { '/' };
	#endregion

	#region Protected Properties		
	/// <summary>
	/// Gets the log.
	/// </summary>
	protected ILogger Logger { get; }
	#endregion

	#region Constructors		
	/// <summary>
	/// Initializes a new instance of the <see cref="DynamicImageUtility"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	public DynamicImageUtility(ILogger<DynamicImageUtility> logger)
	{
		Logger = logger;
	}
	#endregion

	#region Public Methods		
	/// <inheritdoc />
	public virtual DynamicImageFormat ParseImageFormat(string format)
	{
		Guard.IsNotNullOrWhiteSpace(format, nameof(format));

		try
		{
			ReadOnlySpan<char> formatSpan = format.AsSpan().TrimStart('.').Trim();
			Span<char> target = formatSpan.Length <= StackAllocConstants.MaxCharSize ? stackalloc char[formatSpan.Length] : new char[formatSpan.Length];
			formatSpan.ToLowerInvariantSlim(target);

			return target switch
			{
				var _ when target.SequenceEqual("png".AsSpan()) => DynamicImageFormat.Png,
				var _ when target.SequenceEqual("bmp".AsSpan()) => DynamicImageFormat.Bmp,
				var _ when target.SequenceEqual("jpg".AsSpan()) => DynamicImageFormat.Jpeg,
				var _ when target.SequenceEqual("jpeg".AsSpan()) => DynamicImageFormat.Jpeg,
				var _ when target.SequenceEqual("gif".AsSpan()) => DynamicImageFormat.Gif,
				var _ when target.SequenceEqual("webp".AsSpan()) => DynamicImageFormat.WebP,
				_ => DynamicImageFormat.Jpeg
			};
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { format }))
		{
			throw new DynamicImageException("There has been a problem parsing the image format.", exc);
		}
	}

	/// <inheritdoc />
	public virtual (DynamicImageParseUrlResult status, DynamicImageOptions imageOptions) TryParseUrl(string dynamicImagePathPrefix, string relativeUrl, DynamicImageFormat? overrideFormat = null)
	{
		Guard.IsNotNullOrWhiteSpace(dynamicImagePathPrefix);
		Guard.IsNotNullOrWhiteSpace(relativeUrl);

		try
		{
			string url = relativeUrl.TrimToLowerInvariant();

			// Strip away any QueryString
			if (url.Contains('?'))
				url = url.Substring(0, url.IndexOf('?'));

			if (!Path.HasExtension(url))
				return (DynamicImageParseUrlResult.Invalid, default);

			string pathPrefix = dynamicImagePathPrefix.TrimToLowerInvariant();

			if (string.IsNullOrEmpty(url) || !url.StartsWith($"/{pathPrefix}/"))
				return _skipParseUrlResult;

			string[] prefixSegments = pathPrefix.Split(_segmentSeparatorArray, StringSplitOptions.RemoveEmptyEntries);
			string[] allSegments = url.Split(_segmentSeparatorArray, StringSplitOptions.RemoveEmptyEntries);

			if (allSegments.Length - prefixSegments.Length < 5)
				return _invalidParseUrlResult;

			//Ignore the prefix segments
			_ = int.TryParse(allSegments[prefixSegments.Length], out int width);
			_ = int.TryParse(allSegments[prefixSegments.Length + 1], out int height);

			if (width <= 0 || height <= 0)
				return _invalidParseUrlResult;

			DynamicResizeMode mode = allSegments[prefixSegments.Length + 2].ToEnum<DynamicResizeMode>();
			string originalExtension = allSegments[prefixSegments.Length + 3];

			//The extension of this path is the target format the image will be resized as.
			string path = "/" + string.Join("/", allSegments.Skip(prefixSegments.Length + 4));
			string targetExtension = Path.GetExtension(path);

			string sourcePath = Path.ChangeExtension(path, "." + originalExtension);

			//Parse the sourcePath for the pixel density information here
			string pathWithoutExtension = Path.GetFileNameWithoutExtension(path);

			//Check to see if the path has a density identifier at the end
			Match densityMatch = _densityRegex.Match(pathWithoutExtension);

			if (densityMatch.Success)
			{
				//Get the density from the 2nd group
				if (densityMatch.Groups.Count == 2)
				{
					int density = int.Parse(densityMatch.Groups[1].Value);
					int densityIdentifierLength = densityMatch.Value.Length;

					//Remove the density identifier from the path
					string extension = Path.GetExtension(sourcePath);
					int charsToRemove = extension.Length + densityIdentifierLength;

					sourcePath = sourcePath.Remove(sourcePath.Length - charsToRemove, charsToRemove) + extension;

					//Double the dimensions
					width *= density;
					height *= density;
				}
			}

			var imageOptions = new DynamicImageOptions(sourcePath, width, height, mode, overrideFormat ?? ParseImageFormat(targetExtension));

			return (DynamicImageParseUrlResult.Success, imageOptions);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { dynamicImagePathPrefix, relativeUrl }))
		{
			return _invalidParseUrlResult;
		}
	}

	/// <inheritdoc />
	public virtual bool ImageOptionsValid(DynamicImageOptions imageOptions, IEnumerable<DynamicImageMapping> validMappings)
	{
		try
		{
			var mapping = (DynamicImageMapping)imageOptions;

			return validMappings.Contains(mapping);
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { imageOptions, validMappings }))
		{
			throw new DynamicImageException("An error has occurred whilst validating the image options.", exc);
		}
	}

	/// <inheritdoc />
	public virtual string GenerateVirtualPath(string dynamicImagePathPrefix, DynamicImageOptions options)
	{
		Guard.IsNotNullOrWhiteSpace(dynamicImagePathPrefix, nameof(dynamicImagePathPrefix));

		try
		{
			string path = options.SourcePath.Replace("~/", "").TrimStart('/');

			// Remove the querystring and append to the end of the generated URL.
			string? qs = null;

			if (path.Contains('?'))
			{
				string[] parts = path.Split('?');

				if (parts.Length != 2)
					throw new Exception("The path contains more than one '?'.");

				path = parts[0];
				qs = parts[1];
			}

			string originalExtension = Path.GetExtension(path).ToLower().Remove(0, 1);

			string virtualPath = string.Format(VirtualPathFormat,
				dynamicImagePathPrefix,
				options.Width,
				options.Height,
				options.ResizeMode,
				originalExtension,
				path.Replace(originalExtension, options.Format.ToFileExtensionString()).ToLowerInvariant());

			if (!string.IsNullOrEmpty(qs))
				virtualPath += "?" + qs;

			return virtualPath;
		}
		catch (Exception exc) when (Logger.WriteError(exc, new { dynamicImagePathPrefix, options }))
		{
			throw new DynamicImageException("An error has occurred whilst generating the virtual path.", exc);
		}
	}
	#endregion
}