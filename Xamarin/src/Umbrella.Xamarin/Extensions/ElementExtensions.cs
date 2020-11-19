using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Extensions
{
	public static class ElementExtensions
	{
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

		public static IReadOnlyCollection<T> FindPageControls<T>(this Element container, Func<T, bool>? elementSelector = null)
		{
			ILayoutController? layoutController = container.FindRootLayout();

			return layoutController?.FindControls(elementSelector).ToArray() ?? Array.Empty<T>();
		}
	}
}