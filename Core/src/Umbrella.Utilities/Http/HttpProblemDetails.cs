using System.Collections.Generic;

namespace Umbrella.Utilities.Http
{
	/// <summary>
	/// A custom Http ProblemDetails type.
	/// </summary>
	public class HttpProblemDetails
	{
		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the detail.
		/// </summary>
		public string Detail { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the instance.
		/// </summary>
		public string Instance { get; set; }

		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		public int? Status { get; set; }

		/// <summary>
		/// Gets or sets the errors.
		/// </summary>
		public IDictionary<string, string[]> Errors { get; set; }

		/// <summary>
		/// Gets or sets the code.
		/// </summary>
		public string Code { get; set; }
	}
}