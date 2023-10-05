namespace Umbrella.AspNetCore.Blazor.Components.EnumFlags;

/// <summary>
/// A data item which is used to render items in the <see cref="UmbrellaEnumFlagsCheckboxGroup{TEnum}"/> component.
/// </summary>
/// <typeparam name="TEnum">The type of the enum.</typeparam>
public sealed record UmbrellaEnumFlagsCheckboxGroupItem<TEnum>
	where TEnum : struct, Enum
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaEnumFlagsCheckboxGroupItem{TEnum}"/> class.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="text">The text.</param>
	/// <param name="isSelected">Specifies whether this item should be in a selected state.</param>
	/// <param name="isAllOption">Specifies whether this item is the one that can be used to select or deselect all other items.</param>
	public UmbrellaEnumFlagsCheckboxGroupItem(TEnum value, string text, bool isSelected, bool isAllOption)
	{
		Value = value;
		Text = text;
		IsSelected = isSelected;
		IsAllOption = isAllOption;
	}

	/// <summary>
	/// Gets the value.
	/// </summary>
	public TEnum Value { get; }

	/// <summary>
	/// Gets the text shown in the UI.
	/// </summary>
	public string Text { get; }

	/// <summary>
	/// Gets or sets a value indicating whether this instance is selected.
	/// </summary>
	public bool IsSelected { get; set; }

	/// <summary>
	/// Gets a value indicating whether this instance is the option used to select or deselect all other items.
	/// </summary>
	public bool IsAllOption { get; }
}