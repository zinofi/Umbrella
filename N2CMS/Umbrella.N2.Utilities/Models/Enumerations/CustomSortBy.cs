using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Umbrella.N2.BaseModels.Enumerations
{
    public enum CustomSortBy
    {
        Title = 0,
        TitleDescending = 1,
        Published = 2,
        PublishedDescending = 3,
        Updated = 4,
        UpdatedDescending = 5,
        SortOrder = 6,
        Unordered = 7
    }
}