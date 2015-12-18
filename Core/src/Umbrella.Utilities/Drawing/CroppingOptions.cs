using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbrella.Utilities.Drawing
{
    public struct CroppingOptions
    {
        public double CroppingX1 { get; set; }
        public double CroppingY1 { get; set; }
        public double CroppingX2 { get; set; }
        public double CroppingY2 { get; set; }

        /// <summary>
        /// The width of the cropped image area - we don't need the height
        /// This is just used to determine the scaling factor if the image being cropped was scaled down
        /// before being shown to the user when dealing with very large images
        /// </summary>
        public int CroppingWidth { get; set; }
    }
}