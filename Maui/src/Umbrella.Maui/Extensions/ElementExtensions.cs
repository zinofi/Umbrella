using Microsoft.Maui.Controls;

namespace Umbrella.Maui.Extensions;

/// <summary>
/// Extension methods for use with the <see cref="Element"/> type.
/// </summary>
public static class ElementExtensions
{
	/// <summary>
	/// Finds all the controls of type <typeparamref name="T"/> on the current page that the specified <paramref name="element"/> exists on.
	/// </summary>
	/// <typeparam name="T">The type of element to find.</typeparam>
	/// <param name="element">The element.</param>
	/// <param name="elementSelector">The optional element selector.</param>
	/// <returns>An <see cref="IReadOnlyCollection{T}"/> of elements of type <typeparamref name="T"/>.</returns>
	public static IReadOnlyCollection<T> FindPageControls<T>(this Element element, Func<T, bool>? elementSelector = null)
		where T : Element
	{
		var page = element.FindCurrentPage();

		if (page is null)
			return [];

		return [.. GetDescendants(page, elementSelector)];
	}

	/// <summary>
	/// Gets all the descendants of the specified <paramref name="root"/> element that are of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of element to find.</typeparam>
	/// <param name="root">The root element.</param>
	/// <param name="elementSelector">The optional element selector.</param>
	/// <returns>An <see cref="IEnumerable{T}"/> of elements of type <typeparamref name="T"/>.</returns>
	public static IEnumerable<T> GetDescendants<T>(Element root, Func<T, bool>? elementSelector = null)
		where T : Element
	{
		if (root is IElementController elementController)
		{
			foreach (var child in elementController.LogicalChildren.OfType<Element>())
			{
				if (child is T typedChild && (elementSelector is null || elementSelector(typedChild)))
					yield return typedChild;

				foreach (var descendant in GetDescendants<T>(child))
				{
					yield return descendant;
				}
			}
		}
	}

	/// <summary>
	/// Finds the current page on which the specified <paramref name="element"/> exists.
	/// </summary>
	/// <param name="element">The element.</param>
	/// <returns>The current page or <see langword="null"/> if it cannot be found.</returns>
	public static Page? FindCurrentPage(this Element element)
	{
		Element root = element;

		do
		{
			if (root is Page page)
				return page;

			root = root.Parent;
		}
		while (root is not null);

		return null;
	}
}