using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Umbrella.Utilities.Data.Pagination
{
	/// <summary>
	/// A model that encapsulates available pagination options that can be applied to a collection.
	/// </summary>
	[StructLayout(LayoutKind.Auto)]
	public readonly struct PaginationModel
	{
		/// <summary>
		/// An individual paginated page.
		/// </summary>
		[StructLayout(LayoutKind.Auto)]
		public readonly struct PageItem
		{
			/// <summary>
			/// Gets the page number.
			/// </summary>
			public int Number { get; }

			/// <summary>
			/// Gets a value indicating whether this page is the current page.
			/// </summary>
			public bool IsCurrentPage { get; }

			/// <summary>
			/// Initializes a new instance of the <see cref="PageItem"/> struct.
			/// </summary>
			/// <param name="number">The number.</param>
			/// <param name="isCurrentPage">Specifies if this is the current page.</param>
			public PageItem(int number, bool isCurrentPage)
			{
				Number = number;
				IsCurrentPage = isCurrentPage;
			}
		}

		/// <summary>
		/// Gets the first page number.
		/// </summary>
		public int? FirstPageNumber { get; }

		/// <summary>
		/// Gets the previous page number.
		/// </summary>
		public int? PreviousPageNumber { get; }

		/// <summary>
		/// Gets the next page number.
		/// </summary>
		public int? NextPageNumber { get; }

		/// <summary>
		/// Gets the last page number.
		/// </summary>
		public int? LastPageNumber { get; }

		/// <summary>
		/// Gets the total count.
		/// </summary>
		public int TotalCount { get; }

		/// <summary>
		/// Gets a value indicating whether page size selection is enabled.
		/// </summary>
		public bool EnablePageSizeSelection { get; }

		/// <summary>
		/// Gets all the <see cref="PageItem"/>s in the model.
		/// </summary>
		public IReadOnlyCollection<PageItem> PageNumbers { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PaginationModel"/> struct.
		/// </summary>
		/// <param name="totalItems">The total items.</param>
		/// <param name="pageNo">The page no.</param>
		/// <param name="pageSize">Size of the page.</param>
		/// <param name="enablePageSizeSelection">Specified whether page size selection is enabled</param>
		/// <param name="maxPagesToShow">The maximum pages to show.</param>
		public PaginationModel(int totalItems, int pageNo, int? pageSize, bool enablePageSizeSelection = false, int? maxPagesToShow = 5)
			: this()
		{
			TotalCount = totalItems;
			PageItem[] pageItems = null;

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

				pageItems = new PageItem[maxPages];

				for (int i = 0; i < maxPages; i++)
				{
					int pageNumber = startPageNo++;

					pageItems[i] = new PageItem(pageNumber, pageNumber == pageNo);
				}
			}

			PageNumbers = pageItems ?? Array.Empty<PageItem>();
			EnablePageSizeSelection = enablePageSizeSelection;
		}
	}
}