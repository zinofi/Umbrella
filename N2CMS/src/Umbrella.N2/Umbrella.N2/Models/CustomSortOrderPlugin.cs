using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Plugin;
using N2;
using N2.Persistence;
using N2.Engine;
using Umbrella.N2.BaseModels.Enumerations;

namespace Umbrella.N2.BaseModels
{
    /// <summary>
    /// This plugin overrides the sorting mechanism for pages to use the custom one we have built
    /// </summary>
    [AutoInitialize]
    public class CustomSortOrderPlugin : IPluginInitializer
    {
        /// <summary>
        /// Method called by the N2 Framework to initialize the plugin
        /// </summary>
        /// <param name="engine">Instance of the N2 engine</param>
        public void Initialize(IEngine engine)
        {
            //We need to override the sorting of pages here
            engine.Persister.ItemSaved += (obj, args) =>
            {
                //Check the parent of the current page is of type PageModelBase
                if (args.AffectedItem.Parent != null && args.AffectedItem.Parent is PageModelBase)
                {
                    PageModelBase parent = (PageModelBase)args.AffectedItem.Parent;

                    //If we are just going by how they're sorted, using the N2 sorting controls, don't need 
                    //to do anything else here
                    if (parent.CustomSortMethod != CustomSortBy.SortOrder)
                    {
                        List<ContentItem> lstContentItems = null;

                        switch (parent.CustomSortMethod)
                        {
                            case CustomSortBy.Published:
                                lstContentItems = parent.Children.OrderBy(x => x.Published).ToList();
                                break;
                            case CustomSortBy.PublishedDescending:
                                lstContentItems = parent.Children.OrderByDescending(x => x.Published).ToList();
                                break;
                            case CustomSortBy.Title:
                                lstContentItems = parent.Children.OrderBy(x => x.Title).ToList();
                                break;
                            case CustomSortBy.TitleDescending:
                                lstContentItems = parent.Children.OrderByDescending(x => x.Title).ToList();
                                break;
                            case CustomSortBy.Unordered:
                                lstContentItems = Enumerable.Empty<ContentItem>().ToList();
                                break;
                            case CustomSortBy.Updated:
                                lstContentItems = parent.Children.OrderBy(x => x.Updated).ToList();
                                break;
                            case CustomSortBy.UpdatedDescending:
                                lstContentItems = parent.Children.OrderByDescending(x => x.Updated).ToList();
                                break;
                            default:
                                throw new ArgumentException("Unknown Sort Order: " + parent.CustomSortMethod);
                        }

                        parent.Children.Clear();

                        foreach (ContentItem item in lstContentItems)
                            parent.Children.Add(item);

                        lstContentItems = Utility.UpdateSortOrder(lstContentItems).ToList();

                        using (ITransaction transaction = Context.Current.Persister.Repository.BeginTransaction())
                        {
                            foreach (ContentItem item in lstContentItems)
                                Context.Current.Persister.Repository.SaveOrUpdate(item);

                            Context.Current.Persister.Repository.SaveOrUpdate(parent);

                            transaction.Commit();
                        }
                    }
                }
            };
        }
    }
}