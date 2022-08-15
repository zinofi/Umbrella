using Microsoft.AspNetCore.Mvc;

namespace Umbrella.AspNetCore.WebUtilities.Mvc
{
	/// <summary>
	/// An extension of the <see cref="ProblemDetails"/> type with additional properties.
	/// </summary>
	/// <seealso cref="ProblemDetails" />
	public class UmbrellaProblemDetails : ProblemDetails
	{
		/// <summary>
		/// Gets or sets a specific error code which can be used by the client
		/// to handle the error more precisely.
		/// </summary>
		public string? Code { get; set; }

		/// <summary>
		/// Gets or sets the correlation id that can be used to identify the details
		/// associated with this problem in logs.
		/// </summary>
		public string? CorrelationId { get; set; }
	}
}