using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.Components;

namespace Umbrella.AspNetCore.Blazor.Components.Breadcrumb;

/// <summary>
/// A non-rendering breadcrumb component used to specify the information that should be rendered by the <see cref="UmbrellaBreadcrumbRenderer"/>.
/// This component should have one or more <see cref="UmbrellaBreadcrumbItem"/> child components.
/// </summary>
/// <seealso cref="ComponentBase" />
public partial class UmbrellaBreadcrumb
{
	/// <summary>
	/// Gets or sets the child content of this component. This should be a collection of <see cref="UmbrellaBreadcrumbItem"/> components.
	/// </summary>
	[Parameter]
	[EditorRequired]
	public RenderFragment ChildContent { get; set; } = null!;

	/// <summary>
	/// Gets or sets the breadcrumb item list. This property value is provided by Blazor at runtime.
	/// </summary>
	[CascadingParameter]
	public List<UmbrellaBreadcrumbItem> BreadcrumbItemList { get; set; } = null!;

	/// <summary>
	/// Rerenders this instance.
	/// </summary>
	public void Rerender() => WeakReferenceMessenger.Default.Send(new UmbrellaBreadcrumbStateChangedMessage(BreadcrumbItemList));

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		base.OnInitialized();

		BreadcrumbItemList.Clear();
	}

	/// <inheritdoc />
	protected override void OnAfterRender(bool firstRender)
	{
		base.OnAfterRender(firstRender);

		if (firstRender)
			WeakReferenceMessenger.Default.Send(new UmbrellaBreadcrumbStateChangedMessage(BreadcrumbItemList));
	}
}