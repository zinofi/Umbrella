using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Legacy.WebUtilities.DynamicImage.Configuration
{
	public class DynamicImageMappingElementCollection : ConfigurationElementCollection
	{
		#region Overridden Properties
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		protected override string ElementName
		{
			get { return "mapping"; }
		}
		#endregion

		#region Public Overridden Methods
		protected override ConfigurationElement CreateNewElement()
		{
			return new DynamicImageMappingElement();
		}

		protected override Object GetElementKey(ConfigurationElement element)
		{
			return ((DynamicImageMappingElement)element).ToString();
		}

		protected override void BaseAdd(ConfigurationElement element)
		{
			BaseAdd(element, false);
		}
		#endregion

		#region Indexers
		public DynamicImageMappingElement this[int index]
		{
			get { return (DynamicImageMappingElement)BaseGet(index); }
		}

		public new DynamicImageMappingElement this[string Name]
		{
			get { return (DynamicImageMappingElement)BaseGet(Name); }
		}
		#endregion

		#region Public Instance Methods
		public int IndexOf(DynamicImageMappingElement setting)
		{
			return BaseIndexOf(setting);
		}

		public void Add(DynamicImageMappingElement setting)
		{
			BaseAdd(setting);
		}

		public void Remove(DynamicImageMappingElement setting)
		{
			if (BaseIndexOf(setting) >= 0)
				BaseRemove(setting.ToString());
		}

		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}

		public void Remove(string name)
		{
			BaseRemove(name);
		}

		public void Clear()
		{
			BaseClear();
		}
		#endregion
	}
}