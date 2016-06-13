using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Umbrella.Utilities.Drawing
{
    public static class ImageCropper
    {
        public static Bitmap CropImage(Bitmap original, CroppingOptions options)
        {
            double scaleFactor = (double)original.Width / (double)options.CroppingWidth;

            int origWidth = original.Width;
            int origHeight = original.Height;

            //scale input dimensions
            int offsetLeft = (int)Math.Floor(options.CroppingX1 * scaleFactor);
            int offsetTop = (int)Math.Floor(options.CroppingY1 * scaleFactor);
            int newWidth = (int)Math.Floor((options.CroppingX2 - options.CroppingX1) * scaleFactor);
            int newHeight = (int)Math.Floor((options.CroppingY2 - options.CroppingY1) * scaleFactor);

            using (Image resultImg = new Bitmap(newWidth, newHeight))
            {
                using (Graphics graphics = Graphics.FromImage(resultImg))
                {
                    //Make sure the cropArea doesn't exceed the original image width and height
                    int widthCheck = origWidth - (offsetLeft + newWidth);
                    int heightCheck = origHeight - (offsetTop + newHeight);

                    if (widthCheck < 0)
                        offsetLeft += widthCheck;

                    if (heightCheck < 0)
                        offsetTop += heightCheck;

                    Rectangle cropArea = new Rectangle(offsetLeft, offsetTop, newWidth, newHeight);
                    using (Bitmap crop = original.Clone(cropArea, original.PixelFormat))
                    {
                        Bitmap output = ImageResizer.ResizeImage(crop, newWidth, newHeight, ImageResizer.ResizeOptions.ExactWidthAndHeight);
                        return output;
                    }
                }
            }
        }
    }
}
