namespace Umbrella.Utilities.ObjectModel.Abstractions;

/// <summary>
/// A type that represents something that can be selected. Usually used in UI projects
/// where a user can select from a range of options using, e.g. checkboxes.
/// </summary>
/// <typeparam name="TOption">The type of the option.</typeparam>
public interface IUmbrellaSelectableOption<out TOption>
{
	/// <summary>
	/// Specifies whether or not the option is collapsed. If <see langword="true"/>, the children will not be displayed.
	/// </summary>
	bool IsCollapsed { get; set; }

	/// <summary>
	/// Specifies whether or not the option is collapsible. If <see langword="true"/>, the user can toggle the collapsed state of the option.
	/// </summary>
	bool IsCollapsible { get; }

	/// <summary>
	/// Specifies whether or not this instance is selected.
	/// </summary>
	bool IsSelected { get; set; }

	/// <summary>
	/// The text that describes the option. This is usually displayed in the UI to the user.
	/// </summary>
	string Text { get; }

	/// <summary>
	/// The value of the option. This is the actual data that is associated with the option and can be of any type.
	/// </summary>
	TOption Value { get; }

	/// <summary>
	/// Gets a value indicating whether all descendant options are selected.
	/// </summary>
	/// <returns>A value indicating whether all descendant options are selected.</returns>
	bool AllDescendantSelected();
}