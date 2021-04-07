namespace Umbrella.AspNetCore.Blazor.Components.Pagination
{
	/// <summary>
	/// The event arguments for pagination events raised by the <see cref="UmbrellaPagination"/> component.
	/// </summary>
	public readonly struct UmbrellaPaginationEventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaPaginationEventArgs"/> struct.
		/// </summary>
		/// <param name="pageSize">Size of the page.</param>
		/// <param name="pageNumber">The page number.</param>
		public UmbrellaPaginationEventArgs(int pageSize, int pageNumber)
		{
			PageSize = pageSize;
			PageNumber = pageNumber;
		}

		/// <summary>
		/// Gets the size of the page.
		/// </summary>
		public int PageSize { get; }

		/// <summary>
		/// Gets the page number.
		/// </summary>
		public int PageNumber { get; }
	}
}