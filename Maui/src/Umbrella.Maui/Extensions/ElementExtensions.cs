using Microsoft.Maui.Controls;

namespace Umbrella.Maui.Extensions;

/// <summary>
/// Extension methods for use with the <see cref="Element"/> type.
/// </summary>
public static class ElementExtensions
{
	/// <summary>
	/// Finds the root <see cref="ILayoutController" /> for the specified <paramref name="element"/>.
	/// </summary>
	/// <param name="element">The element.</param>
	/// <returns>The root <see cref="ILayoutController"/>.</returns>
	public static ILayoutController? FindRootLayout(this Element element)
	{
		Element root = element;
		ILayoutController? rootLayoutContainer = null;

		do
		{
			if (root is ILayoutController vc)
				rootLayoutContainer = vc;

			root = root.Parent;
		}
		while (root is not null);

		return rootLayoutContainer;
	}

	/// <summary>
	/// Finds the all controls on the page on which the specified <paramref name="container"/> exists.
	/// The root <see cref="ILayoutController"/> for the <paramref name="container"/> is first located, and then all controls
	/// of type <typeparamref name="T"/> are located, optionally filtering them using the specified <paramref name="elementSelector"/>.
	/// </summary>
	/// <typeparam name="T">The type of control to find.</typeparam>
	/// <param name="container">The container.</param>
	/// <param name="elementSelector">The element selector.</param>
	/// <returns>The collection of controls that have been found.</returns>
	public static IReadOnlyCollection<T> FindPageControls<T>(this Element container, Func<T, bool>? elementSelector = null)
	{
		ILayoutController? layoutController = container.FindRootLayout();

		return layoutController?.FindControls(elementSelector).ToArray() ?? [];
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