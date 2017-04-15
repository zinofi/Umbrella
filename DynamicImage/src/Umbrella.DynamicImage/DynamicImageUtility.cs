using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DynamicImage
{
    public class DynamicImageUtility : IDynamicImageUtility
    {
        private static readonly (DynamicImageParseUrlResult, DynamicImageOptions) s_InvalidParseUrlResult = (DynamicImageParseUrlResult.Invalid, default(DynamicImageOptions));
        private static readonly (DynamicImageParseUrlResult, DynamicImageOptions) s_SkipParseUrlResult = (DynamicImageParseUrlResult.Skip, default(DynamicImageOptions));

        #region Private Static Members
        private static readonly Regex s_DensityRegex = new Regex("@([0-9]*)x$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly char[] s_SegmentSeparatorArray = new[] { '/' };
        #endregion

        #region Private Members
        private readonly ILogger m_Logger;
        private readonly IDynamicImageResizer m_DynamicImageResizer;
        private readonly IDynamicImageUrlGenerator m_DynamicImageUrlGenerator;
        #endregion

        #region Constructors
        public DynamicImageUtility(ILogger<DynamicImageUtility> logger,
            IDynamicImageResizer dynamicImageResizer,
            IDynamicImageUrlGenerator dynamicImageUrlGenerator)
        {
            m_Logger = logger;
            m_DynamicImageResizer = dynamicImageResizer;
            m_DynamicImageUrlGenerator = dynamicImageUrlGenerator;
        }
        #endregion

        #region Public Methods
        public DynamicImageFormat ParseImageFormat(string format)
        {
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
                default:
                    return default(DynamicImageFormat);
            }
        }

        public (DynamicImageParseUrlResult Status, DynamicImageOptions ImageOptions) TryParseUrl(string dynamicImagePathPrefix, string relativeUrl)
        {
            try
            {
                string pathPrefix = dynamicImagePathPrefix.ToLowerInvariant();
                string url = relativeUrl?.Trim()?.ToLowerInvariant();

                if (string.IsNullOrEmpty(url) || !url.StartsWith($"/{pathPrefix}/"))
                    return s_SkipParseUrlResult;

                string[] prefixSegments = pathPrefix.Split(s_SegmentSeparatorArray, StringSplitOptions.RemoveEmptyEntries);
                string[] allSegments = url.Split(s_SegmentSeparatorArray, StringSplitOptions.RemoveEmptyEntries);

                if (allSegments.Length - prefixSegments.Length < 5)
                    return s_InvalidParseUrlResult;

                //Ignore the prefix segments
                int width = int.Parse(allSegments[prefixSegments.Length]);
                int height = int.Parse(allSegments[prefixSegments.Length + 1]);
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

                DynamicImageOptions imageOptions = new DynamicImageOptions
                {
                    Width = width,
                    Height = height,
                    ResizeMode = mode,
                    Format = ParseImageFormat(targetExtension),
                    SourcePath = sourcePath
                };

                return (DynamicImageParseUrlResult.Success, imageOptions);
            }
            catch(Exception exc) when (m_Logger.WriteError(exc, new { dynamicImagePathPrefix, relativeUrl }))
            {
                return s_InvalidParseUrlResult;
            }
        }

        public bool ImageOptionsValid(DynamicImageOptions imageOptions, DynamicImageConfigurationOptions configOptions)
        {
            if(configOptions.Enabled)
            {
                var mapping = (DynamicImageMapping)imageOptions;

                if (!configOptions.Mappings.Contains(mapping))
                    return false;
            }

            return true;
        }
        #endregion
    }
}