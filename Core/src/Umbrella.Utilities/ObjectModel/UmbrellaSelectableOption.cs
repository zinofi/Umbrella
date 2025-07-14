using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Umbrella.Utilities.Helpers;
using Umbrella.Utilities.ObjectModel.Abstractions;

namespace Umbrella.Utilities.ObjectModel;

/// <summary>
/// A type that represents something that can be selected. Usually used in UI projects
/// where a user can select from a range of options using, e.g. checkboxes.
/// </summary>
/// <typeparam name="TOption">The type of the option.</typeparam>
#if NET6_0_OR_GREATER
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
public record UmbrellaSelectableOption<TOption> : IUmbrellaSelectableOption, INotifyPropertyChanged
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

	/// <inheritdoc	/>
	public required TOption Value { get; init; }

	/// <inheritdoc	/>
	public required string Text { get; init; }

	/// <inheritdoc	/>
	public bool IsSelected
	{
		get;
		set => INotifyPropertyChangedHelper.SetProperty(ref field, value, this, PropertyChanged);
	}

	/// <inheritdoc	/>
	public bool IsCollapsible { get; init; }

	/// <inheritdoc	/>
	public bool IsCollapsed
	{
		get;
		set => INotifyPropertyChangedHelper.SetProperty(ref field, value, this, PropertyChanged);
	}

	/// <summary>
	/// The parent option, if one exists. This is useful for hierarchical structures where options can have child options.
	/// </summary>
	public UmbrellaSelectableOption<TOption>? Parent { get; init; }

	/// <summary>
	/// The child options. This is useful for hierarchical structures where options can have child options.
	/// </summary>
	public IReadOnlyCollection<UmbrellaSelectableOption<TOption>> Children { get; set; } = [];

	/// <inheritdoc	/>
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <inheritdoc	/>
	public bool AllDescendantSelected()
	{
		if (Children.Count is 0)
			return IsSelected;

		return Children.All(x => x.AllDescendantSelected());
	}
}