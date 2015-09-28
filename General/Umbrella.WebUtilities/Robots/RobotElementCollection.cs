using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Umbrella.WebUtilities.Robots
{
    public class RobotElementCollection : ConfigurationElementCollection
    {
        #region Overridden Properties
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        protected override string ElementName
        {
            get { return "robot"; }
        }
        #endregion

        #region Public Overridden Methods
        protected override ConfigurationElement CreateNewElement()
        {
            return new RobotElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((RobotElement)element).HostName;
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }
        #endregion

        #region Indexers
        public RobotElement this[int index]
        {
            get { return (RobotElement)BaseGet(index); }
        }

        public new RobotElement this[string Name]
        {
            get { return (RobotElement)BaseGet(Name); }
        }
        #endregion

        #region Public Instance Methods
        public int IndexOf(RobotElement setting)
        {
            return BaseIndexOf(setting);
        }

        public void Add(RobotElement setting)
        {
            BaseAdd(setting);
        }

        public void Remove(RobotElement setting)
        {
            if (BaseIndexOf(setting) >= 0)
                BaseRemove(setting.HostName);
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