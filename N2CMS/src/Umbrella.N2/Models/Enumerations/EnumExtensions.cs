using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Umbrella.N2.BaseModels.Enumerations
{
    public static class EnumExtensions
    {
        public static string ToFriendlyString(this CustomSortBy value)
        {
            switch (value)
            {
                case CustomSortBy.TitleDescending:
                    return "Title Descending";
                case CustomSortBy.PublishedDescending:
                    return "Published Descending";
                case CustomSortBy.UpdatedDescending:
                    return "Updated Descending";
                case CustomSortBy.SortOrder:
                    return "Sort Order";
                default:
                    return value.ToString();
            }
        }
    }
}