using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Plugin;
using N2.Engine;
using Umbrella.N2.CustomProperties.LinkEditor.Items;

namespace Umbrella.N2.CustomProperties.LinkEditor.Extensions
{
    [AutoInitialize]
    public class Initializer : IPluginInitializer
    {
        public void Initialize(IEngine engine)
        {
            //Find all LinkItemCollection property plugins
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => typeof(ILinkItemCollectionPlugin).IsAssignableFrom(x) && x != typeof(ILinkItemCollectionPlugin));

            //Now we need to create an instance of each type of plugin
            List<ILinkItemCollectionPlugin> plugins = types.Select(x => Activator.CreateInstance(x)).OfType<ILinkItemCollectionPlugin>().ToList();

            //We now need to validate the plugins to check that the PluginType property extends ListItemBase
            foreach (ILinkItemCollectionPlugin plugin in plugins)
            {
                if (plugin.PluginType == null || !typeof(LinkItemBase).IsAssignableFrom(plugin.PluginType))
                    throw new ApplicationException("The PluginType of the following plugin must inherit from LinkItemBase: " + plugin.GetType().FullName);

                if (string.IsNullOrEmpty(plugin.Name) || plugin.Name.Contains(" "))
                    throw new ApplicationException("The Name of the following plugin must not contain spaces: " + plugin.GetType().FullName);
            }
            
            //Now add these plugins to the list of plugins for the LinkItemCollection
            LinkItemCollection.PluginList = plugins;
        }
    }
}