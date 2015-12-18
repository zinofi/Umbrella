using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Configuration
{
	public class DynamicImageMappingsSection : ConfigurationSection
    {
        #region Public Properties
		[ConfigurationProperty("enabled", DefaultValue = true)]
		public bool Enabled
		{
			get { return (bool)this["enabled"]; }
		}

        [ConfigurationProperty("mappings")]
        public DynamicImageMappingElementCollection Mappings
        {
			get { return this["mappings"] as DynamicImageMappingElementCollection; }
        }
        #endregion
    }
}
