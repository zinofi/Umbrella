using Microsoft.AspNetCore.Mvc;

namespace Umbrella.AspNetCore.WebUtilities.Mvc
{
	/// <summary>
	/// An extension of the <see cref="ProblemDetails"/> type with additional properties.
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.Mvc.ProblemDetails" />
	public class UmbrellaProblemDetails : ProblemDetails
	{
		/// <summary>
		/// Gets or sets a specific error code which can be used by the client
		/// to handle the error more precisely.
		/// </summary>
		public string? Code { get; set; }
	}
}