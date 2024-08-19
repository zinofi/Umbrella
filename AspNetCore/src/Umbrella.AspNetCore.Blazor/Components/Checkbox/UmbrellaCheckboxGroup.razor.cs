using Microsoft.AspNetCore.Components;
using System.Globalization;
using Umbrella.Utilities.ObjectModel;

namespace Umbrella.AspNetCore.Blazor.Components.Checkbox;

/// <summary>
/// A component used to display a list of options as checkboxes.
/// </summary>
/// <typeparam name="TOption">The type of the option.</typeparam>
public partial class UmbrellaCheckboxGroup<TOption>
{
	private UmbrellaCheckboxGroupItem<TOption>? ShowAllOptionItem { get; set; }
	private List<UmbrellaCheckboxGroupItem<TOption>> OptionsToRender { get; } = [];

	/// <summary>
	/// Gets or sets the options.
	/// </summary>
	[Parameter]
	[EditorRequired]
	public IEnumerable<UmbrellaSelectableOption<TOption>> Options { get; set; } = Array.Empty<UmbrellaSelectableOption<TOption>>();

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
	/// Gets or sets an optional callback that can be used to determine how the option value
	/// is displayed as a string in the UI if the <see cref="UmbrellaSelectableOption{TOption}.Text" /> is <see langword="null"/>.
	/// </summary>
	[Parameter]
	public Func<TOption, string>? OptionDisplayNameSelector { get; set; }

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

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		base.OnInitialized();

		foreach (var option in Options)
		{
			string? optionText = option.Text ?? OptionDisplayNameSelector?.Invoke(option.Value);

			if (string.IsNullOrEmpty(optionText) && typeof(TOption).IsEnum)
			{
				var enumValue = Convert.ChangeType(option.Value, typeof(Enum), CultureInfo.InvariantCulture) as Enum;

				if (enumValue is not null)
					optionText = enumValue.ToDisplayString();
			}

			optionText ??= option.Text?.ToString() ?? "Unknown";

			OptionsToRender.Add(new UmbrellaCheckboxGroupItem<TOption>(option, optionText, false));
		}

		if (ShowAllOption)
		{
			ShowAllOptionItem = new UmbrellaCheckboxGroupItem<TOption>(new UmbrellaSelectableOption<TOption>() { Value = default!, IsSelected = OptionsToRender.All(x => x.Option.IsSelected) }, AllOptionDisplayName, true);

			OptionsToRender.Insert(0, ShowAllOptionItem);
		}
	}

	private async Task OnOptionSelectionChangedAsync(UmbrellaCheckboxGroupItem<TOption> option)
	{
		option.Option.IsSelected = !option.Option.IsSelected;

		if (ShowAllOptionItem is not null && !option.IsAllOption)
		{
			ShowAllOptionItem.Option.IsSelected = OptionsToRender.Where(x => !x.IsAllOption).All(x => x.Option.IsSelected);
		}
		else if (ShowAllOptionItem is not null)
		{
			foreach (var item in OptionsToRender)
			{
				if (item.IsAllOption)
					continue;

				item.Option.IsSelected = option.Option.IsSelected;
			}
		}

		if (OnSelectionChanged.HasDelegate)
			await OnSelectionChanged.InvokeAsync();
	}
}

/// <summary>
/// A data item which is used to render items in the <see cref="UmbrellaCheckboxGroup{TEnum}"/> component.
/// </summary>
/// <typeparam name="TOption">The type of the option.</typeparam>
internal sealed record UmbrellaCheckboxGroupItem<TOption>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaCheckboxGroupItem{TOption}"/> class.
	/// </summary>
	/// <param name="option">The option.</param>
	/// <param name="text">The text.</param>
	/// <param name="isAllOption">Specifies whether this item is the one that can be used to select or deselect all other items.</param>
	public UmbrellaCheckboxGroupItem(UmbrellaSelectableOption<TOption> option, string text, bool isAllOption)
	{
		Option = option;
		Text = text;
		IsAllOption = isAllOption;
	}

	/// <summary>
	/// Gets the value.
	/// </summary>
	public UmbrellaSelectableOption<TOption> Option { get; }

	/// <summary>
	/// Gets the text shown in the UI.
	/// </summary>
	public string Text { get; }

	/// <summary>
	/// Gets a value indicating whether this instance is the option used to select or deselect all other items.
	/// </summary>
	public bool IsAllOption { get; }
}