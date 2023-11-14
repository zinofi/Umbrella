using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.Components;

namespace Umbrella.AspNetCore.Blazor.Components.Breadcrumb;

/// <summary>
/// A component used to render a <see cref="UmbrellaBreadcrumb"/> component.
/// </summary>
/// <seealso cref="ComponentBase" />
public partial class UmbrellaBreadcrumbRenderer
{
	[Inject]
	private NavigationManager Navigation { get; set; } = null!;

	/// <summary>
	/// Gets or sets the root for the breadcrumb displayed as the first item in the breadcrumb, e.g. Home
	/// </summary>
	[Parameter]
	public string Root { get; set; } = null!;

	/// <summary>
	/// Gets or sets the root path of the breadcrumb, e.g. /
	/// </summary>
	[Parameter]
	public string RootPath { get; set; } = null!;

	private List<UmbrellaBreadcrumbItem> BreadcrumbItemList { get; } = [];

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		base.OnInitialized();

		Navigation.LocationChanged += (sender, e) =>
		{
			string currentPath = new Uri(e.Location).GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);

			if (currentPath == RootPath.TrimStart('/'))
				BreadcrumbItemList.Clear();

			StateHasChanged();
		};

		WeakReferenceMessenger.Default.Register<UmbrellaBreadcrumbStateChangedMessage>(this, (_, args) =>
		{
			BreadcrumbItemList.Clear();
			BreadcrumbItemList.AddRange(args.Value);
			BreadcrumbItemList.Last().IsActive = true;

			StateHasChanged();
		});
	}
}