using System;

namespace Umbrella.Utilities.Http
{
	/// <summary>
	/// Represents basic metadata of an HTTP resource.
	/// </summary>
	public class HttpResourceInfo
	{
		/// <summary>
		/// Gets or sets the type of the content.
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		/// Gets or sets the length of the content.
		/// </summary>
		public long ContentLength { get; set; }

		/// <summary>
		/// Gets or sets the last modified date.
		/// </summary>
		public DateTime? LastModified { get; set; }

		/// <summary>
		/// Gets or sets the URL.
		/// </summary>
		public string Url { get; set; }
	}
}