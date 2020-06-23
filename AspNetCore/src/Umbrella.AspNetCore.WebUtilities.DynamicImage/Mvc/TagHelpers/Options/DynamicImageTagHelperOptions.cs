using Umbrella.DynamicImage.Abstractions;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.DynamicImage.Mvc.TagHelpers.Options
{
	/// <summary>
	/// Options for use with Dynamic Image Tag Helpers.
	/// </summary>
	/// <seealso cref="Umbrella.Utilities.Options.Abstractions.ISanitizableUmbrellaOptions" />
	/// <seealso cref="Umbrella.Utilities.Options.Abstractions.IValidatableUmbrellaOptions" />
	public class DynamicImageTagHelperOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
	{
		/// <summary>
		/// Gets or sets the dynamic image path prefix. Defaults to <see cref="DynamicImageConstants.DefaultPathPrefix"/>.
		/// </summary>
		public string DynamicImagePathPrefix { get; set; } = DynamicImageConstants.DefaultPathPrefix;

		/// <inheritdoc />
		public void Sanitize() => DynamicImagePathPrefix = DynamicImagePathPrefix.TrimNull();

		/// <inheritdoc />
		public void Validate() => Guard.ArgumentNotNullOrWhiteSpace(DynamicImagePathPrefix, nameof(DynamicImagePathPrefix));
	}
}