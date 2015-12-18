using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Pagination
{
	public struct PaginationModel
	{
		public struct PageItem
		{
			public int Number { get; set; }
			public bool IsCurrentPage { get; set; }
		}

		public int? FirstPageNumber { get; set; }
		public int? PreviousPageNumber { get; set; }
		public int? NextPageNumber { get; set; }
		public int? LastPageNumber { get; set; }
		public int TotalCount { get; set; }
		public bool EnablePageSizeSelection { get; set; }
		public List<PageItem> PageNumbers { get; set; }

		public PaginationModel(int totalItems, int pageNo, int? pageSize, bool enablePageSizeSelection = false, int maxPagesToShow = 5)
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
				int maxPages = maxPagesToShow;

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

				for (int i = 0; i < maxPages; i++)
				{
					int pageNumber = startPageNo++;

					PageNumbers.Add(new PageItem { Number = pageNumber, IsCurrentPage = pageNumber == pageNo });
				}
			}

			EnablePageSizeSelection = enablePageSizeSelection;
		}
	}
}