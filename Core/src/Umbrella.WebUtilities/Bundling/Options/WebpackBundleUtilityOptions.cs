using System;
using System.IO;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;

namespace Umbrella.WebUtilities.Bundling.Options
{
	/// <summary>
	/// Options for the <see cref="WebpackBundleUtility"/>
	/// </summary>
	/// <seealso cref="BundleUtilityOptions" />
	public class WebpackBundleUtilityOptions : BundleUtilityOptions
	{
		/// <summary>
		/// Gets or sets the name of the manifest json file.
		/// </summary>
		public string ManifestJsonFileName { get; set; } = "manifest";

		/// <summary>
		/// Gets the manifest json file sub path.
		/// </summary>
		public string ManifestJsonFileSubPath { get; private set; }

		/// <summary>
		/// Sanitizes this instance.
		/// </summary>
		public override void Sanitize()
		{
			base.Sanitize();

			if (!string.IsNullOrWhiteSpace(ManifestJsonFileName))
			{
				ManifestJsonFileName = ManifestJsonFileName.TrimToLowerInvariant().Trim('/');

				string extension = Path.GetExtension(ManifestJsonFileName);

				if (string.IsNullOrWhiteSpace(extension))
					ManifestJsonFileName += ".json";

				ManifestJsonFileSubPath = "/" + ManifestJsonFileName;
			}
		}

		/// <summary>
		/// Validates this instance.
		/// </summary>
		/// <exception cref="ArgumentException">The manifest file name must only have an extenion of .json.</exception>
		public override void Validate()
		{
			base.Validate();

			Guard.ArgumentNotNullOrWhiteSpace(ManifestJsonFileName, nameof(ManifestJsonFileName));
			Guard.ArgumentNotNullOrWhiteSpace(ManifestJsonFileSubPath, nameof(ManifestJsonFileSubPath));

			string extension = Path.GetExtension(ManifestJsonFileName);

			if (!string.IsNullOrWhiteSpace(extension) && !extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
				throw new ArgumentException("The manifest file name must only have an extenion of .json.");
		}
	}
}