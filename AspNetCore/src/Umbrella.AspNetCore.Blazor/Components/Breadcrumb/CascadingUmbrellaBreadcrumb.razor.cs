using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Umbrella.AspNetCore.Blazor.Components.Breadcrumb;

/// <summary>
/// A cascading component used to allow access to the <see cref="BreadcrumbItemList"/> from other breadcrumb components.
/// This component should wrap the <see cref="Router"/> component inside App.razor.
/// </summary>
/// <seealso cref="ComponentBase" />
public partial class CascadingUmbrellaBreadcrumb
{
	/// <summary>
	/// Gets the breadcrumb item list.
	/// </summary>
	private List<UmbrellaBreadcrumbItem> BreadcrumbItemList { get; } = new();

	/// <summary>
	/// Gets or sets the child content of this component.
	/// </summary>
	[Parameter]
	public RenderFragment ChildContent { get; set; } = null!;
}