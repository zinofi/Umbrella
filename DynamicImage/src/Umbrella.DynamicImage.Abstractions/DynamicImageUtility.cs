using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities;
using Umbrella.Utilities.Constants;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DynamicImage.Abstractions
{
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
		private static readonly (DynamicImageParseUrlResult, DynamicImageOptions) s_InvalidParseUrlResult = (DynamicImageParseUrlResult.Invalid, default);
		private static readonly (DynamicImageParseUrlResult, DynamicImageOptions) s_SkipParseUrlResult = (DynamicImageParseUrlResult.Skip, default);
		private static readonly Regex s_DensityRegex = new Regex("@([0-9]*)x$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly char[] s_SegmentSeparatorArray = new[] { '/' };
		#endregion

		#region Protected Properties		
		/// <summary>
		/// Gets the log.
		/// </summary>
		protected ILogger Log { get; }
		#endregion

		#region Constructors		
		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicImageUtility"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		public DynamicImageUtility(ILogger<DynamicImageUtility> logger)
		{
			Log = logger;
		}
		#endregion

		#region Public Methods		
		/// <inheritdoc />
		public virtual DynamicImageFormat ParseImageFormat(string format)
		{
			Guard.ArgumentNotNullOrWhiteSpace(format, nameof(format));

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
			catch (Exception exc) when (Log.WriteError(exc, new { format }, returnValue: true))
			{
				throw new DynamicImageException("There has been a problem parsing the image format.", exc);
			}
		}

		/// <inheritdoc />
		public virtual (DynamicImageParseUrlResult status, DynamicImageOptions imageOptions) TryParseUrl(string dynamicImagePathPrefix, string relativeUrl, DynamicImageFormat? overrideFormat = null)
		{
			Guard.ArgumentNotNullOrWhiteSpace(dynamicImagePathPrefix, nameof(dynamicImagePathPrefix));
			Guard.ArgumentNotNullOrWhiteSpace(relativeUrl, nameof(relativeUrl));

			try
			{
				string url = relativeUrl.TrimToLowerInvariant();

				if (!Path.HasExtension(url))
					return (DynamicImageParseUrlResult.Invalid, default);

				string pathPrefix = dynamicImagePathPrefix.TrimToLowerInvariant();

				if (string.IsNullOrEmpty(url) || !url.StartsWith($"/{pathPrefix}/"))
					return s_SkipParseUrlResult;

				string[] prefixSegments = pathPrefix.Split(s_SegmentSeparatorArray, StringSplitOptions.RemoveEmptyEntries);
				string[] allSegments = url.Split(s_SegmentSeparatorArray, StringSplitOptions.RemoveEmptyEntries);

				if (allSegments.Length - prefixSegments.Length < 5)
					return s_InvalidParseUrlResult;

				//Ignore the prefix segments
				int.TryParse(allSegments[prefixSegments.Length], out int width);
				int.TryParse(allSegments[prefixSegments.Length + 1], out int height);

				if (width <= 0 || height <= 0)
					return s_InvalidParseUrlResult;

				DynamicResizeMode mode = allSegments[prefixSegments.Length + 2].ToEnum<DynamicResizeMode>();
				string originalExtension = allSegments[prefixSegments.Length + 3];

				//The extension of this path is the target format the image will be resized as.
				string path = "/" + string.Join("/", allSegments.Skip(prefixSegments.Length + 4));
				string targetExtension = Path.GetExtension(path);

				string sourcePath = Path.ChangeExtension(path, "." + originalExtension);

				//Parse the sourcePath for the pixel density information here
				string pathWithoutExtension = Path.GetFileNameWithoutExtension(path);

				//Check to see if the path has a density identifier at the end
				Match densityMatch = s_DensityRegex.Match(pathWithoutExtension);

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
			catch (Exception exc) when (Log.WriteError(exc, new { dynamicImagePathPrefix, relativeUrl }, returnValue: true))
			{
				return s_InvalidParseUrlResult;
			}
		}

		/// <inheritdoc />
		public virtual bool ImageOptionsValid(DynamicImageOptions imageOptions, IEnumerable<DynamicImageMapping> validMappings)
		{
			try
			{
				var mapping = (DynamicImageMapping)imageOptions;

				if (!validMappings.Contains(mapping))
					return false;

				return true;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { imageOptions, validMappings }, returnValue: true))
			{
				throw new DynamicImageException("An error has occurred whilst validating the image options.", exc);
			}
		}

		/// <inheritdoc />
		public virtual string GenerateVirtualPath(string dynamicImagePathPrefix, DynamicImageOptions options)
		{
			Guard.ArgumentNotNullOrWhiteSpace(dynamicImagePathPrefix, nameof(dynamicImagePathPrefix));

			try
			{
				string originalExtension = Path.GetExtension(options.SourcePath).ToLower().Remove(0, 1);

				string path = options.SourcePath.Replace("~/", "");

				string virtualPath = string.Format(VirtualPathFormat,
					dynamicImagePathPrefix,
					options.Width,
					options.Height,
					options.ResizeMode,
					originalExtension,
					path.Replace(originalExtension, options.Format.ToFileExtensionString()));

				return virtualPath;
			}
			catch (Exception exc) when (Log.WriteError(exc, new { dynamicImagePathPrefix, options }, returnValue: true))
			{
				throw new DynamicImageException("An error has occurred whilst generating the virtual path.", exc);
			}
		}
		#endregion
	}
}