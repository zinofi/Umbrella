using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DynamicImage.Abstractions
{
	// TODO: V3 DESIGN - Add support for .webp images
	// Can either support the extension directly, or possibly look into some kind
	// of trick where if the client says they will Accept webp, we send them a webp instead of a png.
	// Not sure if that's allowed or not so need to find resources and experiment a bit.
	// Will obviously require changes to the DynamicImageMiddleware to support this.
	// Also need to ensure that webp is supported by both FreeImage and SkiaSharp.
	// Consider removing SoundInTheory for V3 seeing as it's not cross-platform and doesn't
	// run properly on Azure.
    public enum DynamicImageFormat
    {
        Bmp = 0,
        Gif = 1,
        Jpeg = 2,
        Png = 3,
		WebP = 4
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
                    return value.ToString().ToLowerInvariant();
            }
        }
    }
}