using N2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.N2.BaseModels;

namespace Umbrella.N2.Utilities
{
    /// <summary>
    /// Convenience class to save on typing the generic parameter when calling the class
    /// </summary>
    internal sealed class SiteSettings : SiteSettings<SiteSettingsPageBase>
    {
    }

    public class SiteSettings<T> where T : SiteSettingsPageBase
    {
        #region Private Static Members
        private static T s_SiteSettingsPage;
        private static object s_Lock = new object();
        #endregion

        #region Public Static Properties
        public static T Instance
        {
            get
            {
                EnsureCache();
                return s_SiteSettingsPage;
            }
        }
        #endregion

        #region Static Constructor
        static SiteSettings()
        {
            Context.Current.Persister.ItemSaved += (sender, e) =>
            {
                if (e.AffectedItem is T)
                    lock (s_Lock)
                        s_SiteSettingsPage = null;
            };
        }
        #endregion

        #region Private Static Methods
        private static void EnsureCache()
        {
            if (s_SiteSettingsPage == null)
            {
                lock (s_Lock)
                {
                    if (s_SiteSettingsPage == null)
                    {
                        List<T> lstSiteSettingsPage = Find.StartPage.Children.OfType<T>().ToList();
                        if (lstSiteSettingsPage.Count == 0)
                            throw new Exception("A site settings page does not exist for the site. Please create one under the home page.");
                        else if (lstSiteSettingsPage.Count > 1)
                            throw new Exception("There are multiple site settings pages for the site. This is not allowed.");

                        s_SiteSettingsPage = lstSiteSettingsPage.First();
                    }
                }
            }
        }
        #endregion
    }
}