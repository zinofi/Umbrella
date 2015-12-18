using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.WebUtilities.DynamicImage.Enumerations
{
    public enum DynamicImageFormat
    {
        Bmp = 0,
        Gif = 1,
        Jpeg = 2,
        Png = 3
    }

    public static class DynamicImageFormatExtensions
    {
        public static string ToFileExtensionString(this DynamicImageFormat value)
        {
            switch (value)
            {
                case DynamicImageFormat.Jpeg:
                    return "jpg";
                default:
                    return value.ToString().ToLower();
            }
        }
    }
}
