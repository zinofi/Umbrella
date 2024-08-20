using System.Diagnostics.CodeAnalysis;

namespace Umbrella.Utilities.ObjectModel;

/// <summary>
/// A type that represents something that can be selected. Usually used in UI projects
/// where a user can select from a range of options using, e.g. checkboxes.
/// </summary>
/// <typeparam name="TOption">The type of the option.</typeparam>
#if NET6_0_OR_GREATER
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
public record UmbrellaSelectableOption<TOption>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaSelectableOption{TOption}"/> class.
	/// </summary>
	public UmbrellaSelectableOption()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaSelectableOption{TOption}"/> class.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="isSelected">Specifies whether or not the option is initially selected.</param>
	/// <param name="text">The text.</param>
	[SetsRequiredMembers]
	public UmbrellaSelectableOption(TOption value, bool isSelected = false, string? text = null)
	{
		Value = value;
		IsSelected = isSelected;
		Text = text;
	}

	/// <summary>
	/// Gets the value.
	/// </summary>
	public required TOption Value { get; init; }

	/// <summary>
	/// Gets or sets the text.
	/// </summary>
	public string? Text { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this instance is selected.
	/// </summary>
	public bool IsSelected { get; set; }
}