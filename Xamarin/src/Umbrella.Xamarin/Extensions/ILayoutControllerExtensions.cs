using Xamarin.Forms;

namespace Umbrella.Xamarin.Extensions;

/// <summary>
/// Extension methods for use with the <see cref="ILayoutController"/> type.
/// </summary>
public static class ILayoutControllerExtensions
{
	/// <summary>
	/// Finds the all controls of type <typeparamref name="T"/> which are descendants of the specified <paramref name="container"/> optionally
	/// restricting matches using the <paramref name="elementSelector"/>.
	/// </summary>
	/// <typeparam name="T">The type of the control to find.</typeparam>
	/// <param name="container">The container.</param>
	/// <param name="elementSelector">The element selector.</param>
	/// <returns>The collection of controls that have been found.</returns>
	public static List<T> FindControls<T>(this ILayoutController container, Func<T, bool>? elementSelector = null)
	{
		var lstView = new List<T>();

		foreach (var child in container.Children)
		{
			if (child is ILayoutController childContainer)
				lstView.AddRange(FindControls(childContainer, elementSelector));

			if (child is T target)
				lstView.Add(target);
		}

		return elementSelector is null ? lstView : lstView.Where(elementSelector).ToList();
	}
}