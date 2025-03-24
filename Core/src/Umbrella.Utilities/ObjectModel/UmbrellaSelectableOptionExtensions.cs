using CommunityToolkit.Diagnostics;

namespace Umbrella.Utilities.ObjectModel;

/// <summary>
/// Extension methods for <see cref="UmbrellaSelectableOption{T}"/>.
/// </summary>
public static class UmbrellaSelectableOptionExtensions
{
	/// <summary>
	/// Get a collection of all the selected values by walking the graph of <see cref="UmbrellaSelectableOption{T}"/> instances.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="options">The options.</param>
	/// <returns>A collection of all the selected values.</returns>
	public static IEnumerable<T> GetSelectedValues<T>(this IEnumerable<UmbrellaSelectableOption<T>> options)
	{
		Guard.IsNotNull(options);

		foreach (var option in options)
		{
			if (option.IsSelected)
				yield return option.Value;

			foreach (var child in GetSelectedValues(option.Children))
				yield return child;
		}
	}

	/// <summary>
	/// Clear all the selected values by walking the graph of <see cref="UmbrellaSelectableOption{T}"/> instances
	/// and setting the <see cref="UmbrellaSelectableOption{T}.IsSelected"/> property to <see langword="false"/>.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="options">The options.</param>
	/// <param name="collapseAll">Specifies whether or not to collapse all the options. Defaults to <see langword="false"/>.</param>
	public static void Clear<T>(this IEnumerable<UmbrellaSelectableOption<T>> options, bool collapseAll = false)
	{
		Guard.IsNotNull(options);

		foreach (var option in options)
		{
			option.IsSelected = false;

			if (collapseAll)
				option.IsCollapsed = true;

			Clear(option.Children, collapseAll);
		}
	}

	/// <summary>
	/// Resets the state of all the options by walking the graph of <see cref="UmbrellaSelectableOption{T}"/> instances,
	/// marking them as not selected and collapsed.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="options">The options.</param>
	public static void Reset<T>(this IEnumerable<UmbrellaSelectableOption<T>> options) => options.Clear(true);

	/// <summary>
	/// Collapse all the options by walking the graph of <see cref="UmbrellaSelectableOption{T}"/> instances
	/// and setting the <see cref="UmbrellaSelectableOption{T}.IsCollapsed"/> property to <see langword="true"/>.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="options">The options.</param>
	public static void Collapse<T>(this IEnumerable<UmbrellaSelectableOption<T>> options)
	{
		Guard.IsNotNull(options);

		foreach (var option in options)
		{
			option.IsCollapsed = true;
			Collapse(option.Children);
		}
	}
}