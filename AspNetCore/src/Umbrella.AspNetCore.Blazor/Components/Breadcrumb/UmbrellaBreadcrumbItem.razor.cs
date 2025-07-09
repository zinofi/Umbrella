namespace Umbrella.AspNetCore.Blazor.Components.Breadcrumb;

/// <summary>
/// Represents a breadcrumb item. This is a non-rendering component and should be specified as a child of the <see cref="UmbrellaBreadcrumb"/> component.
/// </summary>
/// <seealso cref="ComponentBase" />
public partial class UmbrellaBreadcrumbItem
{
	/// <summary>
	/// Gets or sets the name of the breadcrumb item which is rendered.
	/// </summary>
	[Parameter]
	[EditorRequired]
	public string Name { get; set; } = null!;

	/// <summary>
	/// Gets or sets the URL. This is optional and is not required for the last item in the breadcrumb.
	/// </summary>
	[Parameter]
	public string? Url { get; set; }

	[CascadingParameter]
	internal UmbrellaBreadcrumb CascadingBreadcrumb { get; set; } = null!;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		base.OnInitialized();

		CascadingBreadcrumb.BreadcrumbItemList.Add(this);
	}
}