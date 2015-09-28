using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using N2;

namespace Umbrella.N2.CustomProperties.LinkEditor.Extensions
{
    /// <summary>
    /// Used to mark types that are used as plugins for the LinkItemCollection property
    /// The following JavaScript functions are automatically generated for the Plugin based on the Name:
    /// Add{0}_Click(event)
    /// OpenAddEdit{0}Dialog(title)
    /// 
    /// A jQuery dialog function is attached to the control. The control have the client id "divAddEdit{0}"
    /// 
    /// The following 2 JavaScript functions must be manually created as part of the plugin creation process
    /// Save{0}()
    /// ResetAddEdit{0}Dialog()
    /// 
    /// {0} is the Name
    /// </summary>
    public interface ILinkItemCollectionPlugin
    {
        Type PluginType { get; }
        string Name { get; }
        string FriendlyName { get; }
        int DialogWidth { get; }
        LinkItemCollectionUserControlBase GetPluginControl();
    }
}