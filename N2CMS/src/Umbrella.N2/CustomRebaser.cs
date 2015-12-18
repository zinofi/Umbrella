using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Engine;
using N2.Edit.Installation;
using N2.Persistence.Finder;
using N2.Persistence;
using N2.Web;
using N2;
using N2.Details;
using log4net;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.log4net;

namespace Umbrella.N2.Utilities
{
    /// <summary>
    /// This replaces the default implementation of the AppPathRebaser. It has been altered to allow links contained within
    /// LinkItemCollection properties to be rebased properly.
    /// </summary>
    [Service(Replaces = typeof(AppPathRebaser))]
    public class CustomRebaser
    {
        #region Private Static Members
        private static readonly ILog Log = LogManager.GetLogger(typeof(CustomRebaser));
        #endregion

        IItemFinder finder;
		IPersister persister;
		IHost host;

        public CustomRebaser(IItemFinder finder, IPersister persister, IHost host)
		{
			this.finder = finder;
			this.persister = persister;
			this.host = host;
		}

		/// <summary>
		/// Rebases all items.
		/// </summary>
		/// <param name="fromUrl"></param>
		/// <param name="toUrl"></param>
		/// <returns></returns>
		/// <remarks>The return enumeration must be enumerated in order for the changes to take effect.</remarks>
		public IEnumerable<RebaseInfo> Rebase(string fromUrl, string toUrl)
		{
            try
            {
                List<RebaseInfo> lstRebaseInfo = new List<RebaseInfo>();

                using (var tx = persister.Repository.BeginTransaction())
                {
                    foreach (var item in finder.All.Select())
                    {
                        bool changed = false;
                        foreach (var info in Rebase(item, fromUrl, toUrl))
                        {
                            changed = true;
                            lstRebaseInfo.Add(info);
                        }
                        if (changed)
                            persister.Repository.SaveOrUpdate(item);
                    }

                    ContentItem root = persister.Get(host.DefaultSite.RootItemID);
                    root[InstallationManager.InstallationAppPath] = toUrl;
                    persister.Repository.SaveOrUpdate(root);

                    persister.Repository.Flush();
                    tx.Commit();
                }

                return lstRebaseInfo;
            }
            catch(Exception exc) when (Log.LogError(exc))
            {
                throw;
            }
		}

		/// <summary>
		/// Rebases a single item.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="fromUrl"></param>
		/// <param name="toUrl"></param>
		/// <returns></returns>
		public static IEnumerable<RebaseInfo> Rebase(ContentItem item, string fromUrl, string toUrl)
		{
            try
            {
                var rebasedLinks = new List<RebaseInfo>();
                foreach (var pi in item.GetContentType().GetProperties())
                {
                    if (pi.CanRead == false || pi.CanWrite == false)
                        continue;

                    foreach (IRelativityTransformer transformer in pi.GetCustomAttributes(typeof(IRelativityTransformer), false))
                    {
                        if (transformer.RelativeWhen != RelativityMode.Always && transformer.RelativeWhen != RelativityMode.Rebasing)
                            continue;

                        string original = item.GetDetail(pi.Name) as string;

                        if (string.IsNullOrEmpty(original))
                            continue;

                        string rebased = transformer.Rebase(original, fromUrl, toUrl);
                        if (!string.Equals(original, rebased))
                        {
                            item.SetDetail(pi.Name, rebased, typeof(string));
                            rebasedLinks.Add(new RebaseInfo { ItemID = item.ID, ItemTitle = item.Title, ItemPath = item.Path, PropertyName = pi.Name });
                        }
                    }
                }
                return rebasedLinks;
            }
            catch(Exception exc) when (Log.LogError(exc))
            {
                throw;
            }
		}
    }
}