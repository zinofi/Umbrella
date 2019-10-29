using System.Collections.Generic;
using System.Linq;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.Legacy.Utilities.Configuration;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Configuration
{
	public class DynamicImageMappingsConfig
	{
		#region Private Static Members
		private static readonly object s_Lock = new object();
		private static List<DynamicImageMapping> s_DynamicImageMappingList;
		private static DynamicImageMappingsSection s_Section;
		#endregion

		#region Constructors
		public DynamicImageMappingsConfig(System.Configuration.Configuration config)
		{
			var group = UmbrellaSectionGroup.GetSectionGroup(config);
			if (group != null)
			{
				s_Section = group.GetConfigurationSection<DynamicImageMappingsSection>("dynamicImageMappings");
			}
		}
		#endregion

		#region Public Properties
		public List<DynamicImageMapping> Settings
		{
			get
			{
				if (s_DynamicImageMappingList == null)
				{
					lock (s_Lock)
					{
						if (s_DynamicImageMappingList == null)
						{
							if (s_Section != null)
							{
								s_DynamicImageMappingList = s_Section.Mappings
									.OfType<DynamicImageMappingElement>()
									.Select(x => new DynamicImageMapping(x.Width, x.Height, x.ResizeMode, x.Format))
									.ToList();
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

		public bool Enabled => s_Section?.Enabled ?? false;
		#endregion

		#region Operators
		public static explicit operator DynamicImageConfigurationOptions(DynamicImageMappingsConfig config) => new DynamicImageConfigurationOptions
		{
			Enabled = config.Enabled,
			Mappings = config.Settings
		};
		#endregion
	}
}