using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Umbrella.Utilities.Drawing
{
    public static class ImageResizer
    {
        public enum ResizeOptions
        {
            // Use fixed width & height without keeping the proportions
            ExactWidthAndHeight,

            // Use maximum width (as defined) and keeping the proportions
            MaxWidth,

            // Use maximum height (as defined) and keeping the proportions
            MaxHeight,

            // Use maximum width or height (the biggest) and keeping the proportions
            MaxWidthAndHeight,

            // Use the target aspect ratio - if required crop the input image - avoids 'white space'
            CropToTargetAspect
        }

        public static System.Drawing.Bitmap DoResize(System.Drawing.Bitmap originalImg, int widthInPixels, int heightInPixels)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(widthInPixels, heightInPixels);

            using (System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(bitmap))
            {
                // Quality properties
                graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                graphic.DrawImage(originalImg, 0, 0, widthInPixels, heightInPixels);
                return bitmap;
            }
        }

        public static System.Drawing.Bitmap ResizeImage(System.Drawing.Bitmap image, int width, int height, ResizeOptions resizeOptions)
        {
            float f_width;
            float f_height;
            float dim;
            switch (resizeOptions)
            {
                case ResizeOptions.ExactWidthAndHeight:
                    return DoResize(image, width, height);

                case ResizeOptions.MaxHeight:
                    f_width = image.Width;
                    f_height = image.Height;

                    if (f_height <= height)
                        return DoResize(image, (int)f_width, (int)f_height);

                    dim = f_width / f_height;
                    width = (int)((float)(height) * dim);
                    return DoResize(image, width, height);

                case ResizeOptions.MaxWidth:
                    f_width = image.Width;
                    f_height = image.Height;

                    if (f_width <= width)
                        return DoResize(image, (int)f_width, (int)f_height);

                    dim = f_width / f_height;
                    height = (int)((float)(width) / dim);
                    return DoResize(image, width, height);

                case ResizeOptions.MaxWidthAndHeight:
                    int tmpHeight = height;
                    int tmpWidth = width;
                    f_width = image.Width;
                    f_height = image.Height;

                    if (f_width <= width && f_height <= height)
                        return DoResize(image, (int)f_width, (int)f_height);

                    dim = f_width / f_height;

                    // Check if the width is ok
                    if (f_width < width)
                        width = (int)f_width;
                    height = (int)((float)(width) / dim);
                    // The width is too width
                    if (height > tmpHeight)
                    {
                        if (f_height < tmpHeight)
                            height = (int)f_height;
                        else
                            height = tmpHeight;
                        width = (int)((float)(height) * dim);
                    }
                    return DoResize(image, width, height);

                case ResizeOptions.CropToTargetAspect:
                    //Calculate aspect ratios for target and image
                    f_width = width;
                    f_height = height;
                    float target_Aspect = f_width / f_height;

                    f_width = image.Width;
                    f_height = image.Height;
                    float image_Aspect = f_width / f_height;

                    // If target aspect ratio > image aspect ratio, image is 'taller' than target - need to crop top and bottom
                    if (target_Aspect > image_Aspect)
                    {
                        int i_height = (int)(image.Width / target_Aspect);
                        Rectangle rect = new Rectangle(0, (image.Height - i_height) / 2, image.Width, i_height);
                        image = image.Clone(rect, image.PixelFormat);
                    }
                    // If target aspect ratio < image aspect ratio, image is 'fatter' than target - need to crop left and right
                    else if (target_Aspect < image_Aspect)
                    {
                        int i_width = (int)(image.Height * target_Aspect);
                        Rectangle rect = new Rectangle((image.Width - i_width) / 2, 0, i_width, image.Height);
                        image = image.Clone(rect, image.PixelFormat);
                    }

                    // Now resize the image as usual
                    return DoResize(image, width, height);

                default:
                    return image;
            }
        }

        public static System.Drawing.Bitmap ResizeImage(Stream originalImage, int width, int height, ResizeOptions resizeOptions)
        {
            Bitmap image = new Bitmap(originalImage);
            return ResizeImage(image, width, height, resizeOptions);
        }

        public static void ResizeImage(string originalImagePath, string outputImagePath, int width, int height, ResizeOptions resizeOptions)
        {
            using (Bitmap originalImage = (Bitmap)Bitmap.FromFile(originalImagePath))
            {
                using (Bitmap resized = ResizeImage(originalImage, width, height, resizeOptions))
                {
                    //Check that the image has a size, the resize may not have worked
                    //Save to a memorystream to check this
                    using (MemoryStream ms = new MemoryStream())
                    {
                        resized.Save(ms, originalImage.RawFormat);

                        //Check the file has a length
                        if (ms.Length > 0)
                        {
                            using (FileStream fs = new FileStream(outputImagePath, FileMode.Create, FileAccess.ReadWrite))
                            {
                                ms.Position = 0;

                                byte[] bytes = new byte[ms.Length];
                                ms.Read(bytes, 0, (int)ms.Length);
                                fs.Write(bytes, 0, bytes.Length);
                            }
                        }
                    }
                }
            }
        }

        public static byte[] ConvertImageToByteArray(Bitmap image, ImageFormat format)
        {
            using (MemoryStream mem = new MemoryStream())
            {
                mem.Position = 0;
                image.Save(mem, format);
                return mem.ToArray();
            }
        }
    }
}
