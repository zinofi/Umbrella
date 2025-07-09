using CommunityToolkit.Diagnostics;
using Umbrella.Utilities.Options.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Components.Breadcrumb.Options;

/// <summary>
/// Options for use with the <see cref="UmbrellaBreadcrumb"/> component.
/// </summary>
public class UmbrellaBreadcrumbOptions : ISanitizableUmbrellaOptions, IValidatableUmbrellaOptions
{
	/// <summary>
	/// Gets or sets the name of the root breadcrumb item.
	/// </summary>
	public string RootName { get; set; } = "Home";

	/// <summary>
	///	Gets or sets the URL of the root breadcrumb item.
	/// </summary>
	public string RootPath { get; set; } = "/";

	/// <inheritdoc />
	public void Sanitize()
	{
		RootName = RootName?.Trim()!;
		RootPath = RootPath?.Trim()!;
	}

	/// <inheritdoc />
	public void Validate()
	{
		Guard.IsNotNullOrEmpty(RootName);
		Guard.IsNotNullOrEmpty(RootPath);
	}
}