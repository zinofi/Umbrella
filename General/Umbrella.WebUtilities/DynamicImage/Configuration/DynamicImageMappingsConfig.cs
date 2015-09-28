using AutoMapper;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Configuration;

namespace Umbrella.WebUtilities.DynamicImage.Configuration
{
	public static class DynamicImageMappingsConfig
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(DynamicImageMappingsConfig));
		private static readonly object s_Lock = new object();
		private static List<DynamicImageMapping> m_DynamicImageMappingList;
		private static DynamicImageMappingsSection m_Section;

		static DynamicImageMappingsConfig()
		{
            UmbrellaSectionGroup group = null; //TODO: UmbrellaSectionGroup.GetSectionGroup(null);
			if (group != null)
			{
				m_Section = group.GetConfigurationSection<DynamicImageMappingsSection>("dynamicImageMappings");
			}
		}

		public static List<DynamicImageMapping> Settings
		{
			get
			{
				if(m_DynamicImageMappingList == null)
				{
					lock(s_Lock)
					{
						if(m_DynamicImageMappingList == null)
						{
							if (m_Section != null)
							{
								m_DynamicImageMappingList = m_Section.Mappings.OfType<DynamicImageMappingElement>().Select(x => Mapper.DynamicMap<DynamicImageMapping>(x)).ToList();
							}

							//If no config settings can be found, initialize as empty
							if (m_DynamicImageMappingList == null)
								m_DynamicImageMappingList = new List<DynamicImageMapping>();
						}
					}
				}

				return m_DynamicImageMappingList;
			}
		}

		public static bool Enabled
		{
			get { return m_Section != null ? m_Section.Enabled : true; }
		}
	}
}
