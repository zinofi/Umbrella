using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Extensions
{
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
			while (root != null);

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

			return layoutController?.FindControls(elementSelector).ToArray() ?? Array.Empty<T>();
		}
	}
}