using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Pagination
{
	public readonly struct PaginationModel
	{
		public readonly struct PageItem
		{
			public int Number { get; }
			public bool IsCurrentPage { get; }

            public PageItem(int number, bool isCurrentPage)
            {
                Number = number;
                IsCurrentPage = isCurrentPage;
            }
		}

		public int? FirstPageNumber { get; }
		public int? PreviousPageNumber { get; }
		public int? NextPageNumber { get; }
		public int? LastPageNumber { get; }
		public int TotalCount { get; }
		public bool EnablePageSizeSelection { get; }
		public List<PageItem> PageNumbers { get; }

		public PaginationModel(int totalItems, int pageNo, int? pageSize, bool enablePageSizeSelection = false, int? maxPagesToShow = 5)
            : this()
		{
			TotalCount = totalItems;
			PageNumbers = new List<PageItem>();

			if (pageSize.HasValue)
			{
				int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

				if (totalPages > 1 && pageNo > 1)
					FirstPageNumber = 1;

				if (totalPages > 1 && pageNo > 1)
					PreviousPageNumber = pageNo - 1;

				if (totalPages > 1 && pageNo < totalPages)
					LastPageNumber = totalPages;

				if (totalPages > 1 && pageNo < totalPages)
					NextPageNumber = pageNo + 1;

				int startPageNo = 1;
                int maxPages = maxPagesToShow ?? totalPages;

                if (maxPagesToShow.HasValue)
                {
                    if (totalPages <= maxPagesToShow)
                    {
                        maxPages = totalPages;
                    }
                    else
                    {
                        int halfway = maxPages / 2;
                        startPageNo = pageNo - halfway;

                        if (startPageNo < halfway)
                            startPageNo = 1;
                        else if (totalPages - halfway < pageNo)
                            startPageNo = totalPages - maxPages + 1;
                    }
                }

				for (int i = 0; i < maxPages; i++)
				{
					int pageNumber = startPageNo++;

					PageNumbers.Add(new PageItem(pageNumber, pageNumber == pageNo));
				}
			}

			EnablePageSizeSelection = enablePageSizeSelection;
		}
	}
}