using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using N2.Collections;
using N2;
using N2.Definitions;

namespace Umbrella.N2.Utilities
{
    public sealed class PageFinder : global::N2.Persistence.GenericFind<ContentItem, ContentItem>
    {
        private static readonly object s_Lock = new object();
        private static Dictionary<Type, string> s_UrlDictionary = new Dictionary<Type, string>();

        static PageFinder()
        {
            Context.Persister.ItemSaved += (obj, args) =>
                {
                    lock (s_Lock)
                        s_UrlDictionary = new Dictionary<Type, string>();
                };
        }

        /// <summary>
        /// Gets the item at the specified level.
        /// </summary>
        /// <param name="level">Level = 1 equals start page, level = 2 a child of the start page, and so on.</param>
        /// <returns>An ancestor at the specified level.</returns>
        public static ContentItem AncestorAtLevel(int level)
        {
            return AncestorAtLevel(level, Parents, CurrentPage);
        }

        /// <summary>
        /// Gets the item at the specified level.
        /// </summary>
        /// <param name="level">Level = 1 equals start page, level = 2 a child of the start page, and so on.</param>
        /// <returns>An ancestor at the specified level.</returns>
        public static ContentItem AncestorAtLevel(int level, IEnumerable<ContentItem> parents, ContentItem currentPage)
        {
            ItemList items = new ItemList(parents);
            if (items.Count >= level)
                return items[items.Count - level];
            else if (items.Count == level - 1)
                return currentPage;
            return null;
        }

        /// <summary>
        /// Returns the URL for the first published instance of the specified page type.
        /// </summary>
        /// <typeparam name="T">The type of content item</typeparam>
        /// <returns>The URL for the first publsihed instance of type T</returns>
        public static string GetPageUrl<T>() where T : ContentItem
        {
            if (!s_UrlDictionary.ContainsKey(typeof(T)))
            {
                lock (s_Lock)
                {
                    if (!s_UrlDictionary.ContainsKey(typeof(T)))
                    {
                        ContentItem startPage = Find.Query<ContentItem>().Where(x => x.ID == Find.StartPage.ID).First();

                        T page = Find.EnumerateChildren(startPage).Where(x => x.State == ContentState.Published).OfType<T>().FirstOrDefault();

                        if(page != null)
                            s_UrlDictionary.Add(typeof(T), page.Url);
                    }
                }
            }

            return s_UrlDictionary.ContainsKey(typeof(T))
                ? s_UrlDictionary[typeof(T)]
                : string.Empty;
        }
    }
}