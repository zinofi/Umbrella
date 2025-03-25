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
	/// <param name="text">The text.</param>
	/// <param name="isSelected">Specifies whether or not the option is initially selected.</param>
	/// <param name="isCollapsible">Specifies whether or not the option is collapsible.</param>
	/// <param name="isCollapsed">Specifies whether or not the option is initially collapsed.</param>
	/// <param name="children">The child options.</param>
	/// <param name="parent">The parent, if one exists.</param>
	[SetsRequiredMembers]
	public UmbrellaSelectableOption(
		TOption value,
		string text,
		bool isSelected = false,
		bool isCollapsible = false,
		bool isCollapsed = true,
		IEnumerable<UmbrellaSelectableOption<TOption>>? children = null,
		UmbrellaSelectableOption<TOption>? parent = null)
	{
		Value = value;
		Text = text;
		IsSelected = isSelected;
		IsCollapsible = isCollapsible;
		IsCollapsed = isCollapsed;
		Parent = parent;

		if (children is not null)
			Children = children.ToArray();
	}

	/// <summary>
	/// Gets the value.
	/// </summary>
	public required TOption Value { get; init; }

	/// <summary>
	/// Gets or sets the text.
	/// </summary>
	public required string Text { get; init; }

	/// <summary>
	/// Gets or sets a value indicating whether this instance is selected.
	/// </summary>
	public bool IsSelected { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this instance is collapsible. Defaults to <see langword="false"/>.
	/// </summary>
	public bool IsCollapsible { get; init; }

	/// <summary>
	/// Gets or sets a value indicating whether this instance is collapsed. Defaults to <see langword="true"/>.
	/// </summary>
	public bool IsCollapsed { get; set; } = true;

	/// <summary>
	/// Gets or sets the parent option.
	/// </summary>
	/// <remarks>
	/// This is useful for when you need to know the parent of a child option.
	/// </remarks>
	public UmbrellaSelectableOption<TOption>? Parent { get; init; }

	/// <summary>
	/// Gets or sets the child options.
	/// </summary>
	public IReadOnlyCollection<UmbrellaSelectableOption<TOption>> Children { get; init; } = [];

	/// <summary>
	/// Gets a value indicating whether all descendant options are selected.
	/// </summary>
	/// <returns>A value indicating whether all descendant options are selected.</returns>
	public bool AllDescendantSelected()
	{
		if (Children.Count is 0)
			return IsSelected;

		return Children.All(x => x.AllDescendantSelected());
	}
}