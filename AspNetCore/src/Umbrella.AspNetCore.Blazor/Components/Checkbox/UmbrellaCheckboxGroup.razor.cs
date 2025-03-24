using Microsoft.AspNetCore.Components;
using Umbrella.Utilities.ObjectModel;

namespace Umbrella.AspNetCore.Blazor.Components.Checkbox;

/// <summary>
/// A component used to display a list of options as checkboxes.
/// </summary>
/// <typeparam name="TOption">The type of the option.</typeparam>
public partial class UmbrellaCheckboxGroup<TOption>
{
	private UmbrellaSelectableOption<TOption>? ShowAllOptionItem { get; set; }
	private List<UmbrellaSelectableOption<TOption>> OptionsToRender { get; } = [];

	/// <summary>
	/// Gets or sets the options.
	/// </summary>
	[Parameter]
	[EditorRequired]
	public IEnumerable<UmbrellaSelectableOption<TOption>> Options { get; set; } = [];

	/// <summary>
	/// Gets or sets the CSS class.
	/// </summary>
	[Parameter]
	public string? CssClass { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether or not there should be an option at the top of the
	/// rendered list of checkboxes that can be used to select or deselect all other options.
	/// </summary>
	[Parameter]
	public bool ShowAllOption { get; set; }

	/// <summary>
	/// Gets or sets the display name of 'All' option.
	/// </summary>
	/// <remarks>
	/// Defaults to <c>All</c>
	/// </remarks>
	[Parameter]
	public string AllOptionDisplayName { get; set; } = "All";

	/// <summary>
	/// Gets or sets the callback that is invoked when an option is selected or deselected.
	/// </summary>
	[Parameter]
	public EventCallback OnSelectionChanged { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether the checkbox group is disabled. This has the effect of setting all checkboxes to disabled.
	/// </summary>
	[Parameter]
	public bool Disabled { get; set; }

	/// <summary>
	/// Gets or sets the text to display when the option is collapsed. Defaults to <c>Show</c>.
	/// </summary>
	[Parameter]
	public string CollapsedToggleText { get; set; } = "Show";

	/// <summary>
	/// Gets or sets the text to display when the option is expanded. Defaults to <c>Hide</c>.
	/// </summary>
	[Parameter]
	public string ExpandedToggleText { get; set; } = "Hide";

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		base.OnInitialized();

		OptionsToRender.AddRange(Options);

		if (ShowAllOption)
		{
			ShowAllOptionItem = new UmbrellaSelectableOption<TOption>()
			{
				Value = default!,
				Text = AllOptionDisplayName,
				IsSelected = OptionsToRender.All(x => x.IsSelected)
			};

			OptionsToRender.Insert(0, ShowAllOptionItem);
		}
	}

	public async Task ClearAsync()
	{
		Options.Clear();

		if (OnSelectionChanged.HasDelegate)
			await OnSelectionChanged.InvokeAsync();
	}

	public async Task ResetAsync()
	{
		Options.Reset();

		if (OnSelectionChanged.HasDelegate)
			await OnSelectionChanged.InvokeAsync();
	}

	public void Collapse() => Options.Collapse();

	private async Task OnOptionSelectionChangedAsync(UmbrellaSelectableOption<TOption> option)
	{
		option.IsSelected = !option.IsSelected;

		if (ShowAllOptionItem is not null && option != ShowAllOptionItem)
		{
			ShowAllOptionItem.IsSelected = OptionsToRender.Where(x => x != ShowAllOptionItem).All(x => x.IsSelected);
		}
		else if (ShowAllOptionItem is not null)
		{
			foreach (var item in OptionsToRender)
			{
				if (item == ShowAllOptionItem)
					continue;

				item.IsSelected = option.IsSelected;
			}
		}

		// TODO: The below behaviour is very specific. This could be included in the UmbrellaSelectableOption class itself
		// but need to consider if this is the best approach. What if a different client wants different behaviour?
		// Just give it a descriptive method name and then we'll know what it does.
		// Need some way of overriding this behaviour if needed. We could have a delegate that is called when the selection changes.
		// and default it.

		// Need to recursively go up and down the tree to update the parent and children.
		// When doing down the tree, select / deselect all children to match this.
		static void UpdateDescendants(UmbrellaSelectableOption<TOption> parent, bool isSelected)
		{
			foreach (var child in parent.Children)
			{
				child.IsSelected = isSelected;
				UpdateDescendants(child, isSelected);
			}
		}

		static void UpdateAncestors(UmbrellaSelectableOption<TOption> child, bool isSelected)
		{
			if (child.Parent is not null)
			{
				child.Parent.IsSelected = child.Parent.AllDescendantSelected();
				UpdateAncestors(child.Parent, isSelected);
			}
		}

		UpdateDescendants(option, option.IsSelected);
		UpdateAncestors(option, option.IsSelected);

		if (OnSelectionChanged.HasDelegate)
			await OnSelectionChanged.InvokeAsync();
	}

	private static void CollapseToggleClick(UmbrellaSelectableOption<TOption> option)
	{
		option.IsCollapsed = !option.IsCollapsed;

		// TODO: Consider moving to an instance method on the UmbrellaSelectableOption class
		static void CollapseAllDescendants(UmbrellaSelectableOption<TOption> parent)
		{
			foreach (var child in parent.Children)
			{
				child.IsCollapsed = true;
				CollapseAllDescendants(child);
			}
		}

		if (option.IsCollapsed)
			CollapseAllDescendants(option);
	}
}