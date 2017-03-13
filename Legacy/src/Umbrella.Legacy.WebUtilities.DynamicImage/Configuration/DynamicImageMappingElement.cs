using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DynamicImage.Abstractions;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Configuration
{
	public class DynamicImageMappingElement : ConfigurationElement
    {
        #region Public Properties
        [ConfigurationProperty("width", IsRequired = true)]
        public int Width
        {
			get { return (int)this["width"]; }
        }

        [ConfigurationProperty("height", IsRequired = true)]
        public int Height
        {
			get { return (int)this["height"]; }
        }

		[ConfigurationProperty("resizeMode", IsRequired = true)]
		public DynamicResizeMode ResizeMode
		{
			get { return (DynamicResizeMode)this["resizeMode"]; }
		}

		[ConfigurationProperty("format", IsRequired = true)]
		public DynamicImageFormat Format
		{
			get { return (DynamicImageFormat)this["format"]; }
		}
        #endregion

		public override string ToString()
		{
			return string.Format("{0}-{1}-{2}-{3}",
				Width,
				Height,
				ResizeMode,
				Format);
		}
    }
}