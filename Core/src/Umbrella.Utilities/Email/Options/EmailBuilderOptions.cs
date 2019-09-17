using System;
using Umbrella.Utilities.Abstractions;

namespace Umbrella.Utilities.Email.Options
{
	/// <summary>
	/// Options for the <see cref="EmailBuilder"/>.
	/// </summary>
	public class EmailBuilderOptions : IValidatableUmbrellaOptions
	{
		/// <summary>
		/// Gets or sets the virtual path of the folder containing the email templates. This defaults to "~/Content/EmailTemplates/".
		/// </summary>
		public string TemplatesVirtualPath { get; set; } = "~/Content/EmailTemplates/";

		/// <summary>
		/// Gets or sets the HTML string template used for data rows. This defaults to using tr and td table elements.
		/// </summary>
		public string DataRowFormat { get; set; } = "<tr><td>{0}:</td><td>{1}</td></tr>";

		/// <summary>
		/// Validates this instance.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <see cref="DataRowFormat"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <see cref="DataRowFormat"/> is empty or whitespace.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <see cref="TemplatesVirtualPath"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <see cref="TemplatesVirtualPath"/> is empty or whitespace.</exception>
		public void Validate()
		{
			Guard.ArgumentNotNullOrWhiteSpace(DataRowFormat, nameof(DataRowFormat));
			Guard.ArgumentNotNullOrWhiteSpace(TemplatesVirtualPath, nameof(TemplatesVirtualPath));
		}
	}
}