using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DynamicImage
{
    public class DynamicImageUtility : IDynamicImageUtility
    {
        #region Private Constants
        private const string c_VirtualPathFormat = "~/{0}/{1}/{2}/{3}/{4}/{5}";
        #endregion

        #region Private Static Members
        private static readonly (DynamicImageParseUrlResult, DynamicImageOptions) s_InvalidParseUrlResult = (DynamicImageParseUrlResult.Invalid, default);
        private static readonly (DynamicImageParseUrlResult, DynamicImageOptions) s_SkipParseUrlResult = (DynamicImageParseUrlResult.Skip, default);
        private static readonly Regex s_DensityRegex = new Regex("@([0-9]*)x$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly char[] s_SegmentSeparatorArray = new[] { '/' };
        #endregion

        #region Protected Properties
        protected ILogger Log { get; }
        #endregion

        #region Constructors
        public DynamicImageUtility(ILogger<DynamicImageUtility> logger)
        {
            Log = logger;
        }
        #endregion

        #region Public Methods
        public virtual DynamicImageFormat ParseImageFormat(string format)
        {
			Guard.ArgumentNotNullOrWhiteSpace(format, nameof(format));

            switch (format?.TrimStart('.').Trim()?.ToLowerInvariant())
            {
                case "png":
                    return DynamicImageFormat.Png;
                case "bmp":
                    return DynamicImageFormat.Bmp;
                case "jpg":
                    return DynamicImageFormat.Jpeg;
                case "gif":
                    return DynamicImageFormat.Gif;
				case "webp":
					return DynamicImageFormat.WebP;
                default:
                    return default;
            }
        }

        public virtual (DynamicImageParseUrlResult status, DynamicImageOptions imageOptions) TryParseUrl(string dynamicImagePathPrefix, string relativeUrl, DynamicImageFormat? overrideFormat = null)
        {
			Guard.ArgumentNotNullOrWhiteSpace(dynamicImagePathPrefix, nameof(dynamicImagePathPrefix));
			Guard.ArgumentNotNullOrWhiteSpace(relativeUrl, nameof(relativeUrl));

			try
            {
                string url = relativeUrl?.TrimToLowerInvariant();

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
            catch(Exception exc) when (Log.WriteError(exc, new { dynamicImagePathPrefix, relativeUrl }, returnValue: true))
            {
                return s_InvalidParseUrlResult;
            }
        }

        public virtual bool ImageOptionsValid(DynamicImageOptions imageOptions, DynamicImageConfigurationOptions configOptions)
        {
            if(configOptions.Enabled)
            {
                var mapping = (DynamicImageMapping)imageOptions;

                if (!configOptions.Mappings.Contains(mapping))
                    return false;
            }

            return true;
        }

        public virtual string GenerateVirtualPath(string dynamicImagePathPrefix, DynamicImageOptions options)
        {
			Guard.ArgumentNotNullOrWhiteSpace(dynamicImagePathPrefix, nameof(dynamicImagePathPrefix));

			try
            {
                string originalExtension = Path.GetExtension(options.SourcePath).ToLower().Remove(0, 1);

                string path = options.SourcePath.Replace("~/", "");

                string virtualPath = string.Format(c_VirtualPathFormat,
                    dynamicImagePathPrefix,
                    options.Width,
                    options.Height,
                    options.ResizeMode,
                    originalExtension,
                    path.Replace(originalExtension, options.Format.ToFileExtensionString()));

                return virtualPath;
            }
            catch (Exception exc) when (Log.WriteError(exc, new { dynamicImagePathPrefix, options }))
            {
                throw;
            }
        }
        #endregion
    }
}