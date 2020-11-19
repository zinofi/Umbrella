using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Extensions
{
	public static class ILayoutControllerExtensions
	{
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
}