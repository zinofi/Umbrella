using AutoMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Legacy.Utilities.Configuration;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Configuration
{
	public class DynamicImageMappingsConfig
	{
		private static readonly object s_Lock = new object();
		private static List<DynamicImageMapping> s_DynamicImageMappingList;
		private static DynamicImageMappingsSection s_Section;

		public DynamicImageMappingsConfig(System.Configuration.Configuration config)
		{
            UmbrellaSectionGroup group = UmbrellaSectionGroup.GetSectionGroup(config);
			if (group != null)
			{
				s_Section = group.GetConfigurationSection<DynamicImageMappingsSection>("dynamicImageMappings");
			}
		}

		public List<DynamicImageMapping> Settings
		{
			get
			{
				if(s_DynamicImageMappingList == null)
				{
					lock(s_Lock)
					{
						if(s_DynamicImageMappingList == null)
						{
							if (s_Section != null)
							{
								s_DynamicImageMappingList = s_Section.Mappings.OfType<DynamicImageMappingElement>().Select(x => Mapper.DynamicMap<DynamicImageMapping>(x)).ToList();
							}

							//If no config settings can be found, initialize as empty
							if (s_DynamicImageMappingList == null)
								s_DynamicImageMappingList = new List<DynamicImageMapping>();
						}
					}
				}

				return s_DynamicImageMappingList;
			}
		}

		public bool Enabled
		{
			get { return s_Section != null ? s_Section.Enabled : true; }
		}
	}
}
