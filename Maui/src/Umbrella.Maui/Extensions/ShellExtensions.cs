﻿using CommunityToolkit.Diagnostics;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace Umbrella.Maui.Extensions;

/// <summary>
/// Extension methods for use with the <see cref="Shell"/> type.
/// </summary>
public static class ShellExtensions
{
	/// <summary>
	/// Navigates to the specified path if the page isn't already on either the
	/// <see cref="INavigation.ModalStack"/> or the <see cref="INavigation.NavigationStack"/> so it isn't duplicated.
	/// Additionally, all other pages currently on either stack are removed to ensure the page at the specified <paramref name="path"/> is always shown.
	/// </summary>
	/// <typeparam name="TPage">The type of the page.</typeparam>
	/// <param name="shell">The shell instance.</param>
	/// <param name="path">The path.</param>
	/// <returns>An awaitable <see cref="ValueTask"/> that completes when the operation has completed.</returns>
	public static async ValueTask GoToRootAsync<TPage>(this Shell shell, string path)
		where TPage : Page
	{
		Guard.IsNotNull(shell);

		if (!shell.Navigation.ModalStack.Any(x => x is TPage) && !shell.Navigation.NavigationStack.Any(x => x is TPage))
		{
			await MainThread.InvokeOnMainThreadAsync(async () =>
			{
				await shell.GoToAsync(path);

				for (int i = 0; i < shell.Navigation.NavigationStack.Count; i++)
				{
					var page = shell.Navigation.NavigationStack[i];

					if (page is null or TPage)
						continue;

					shell.Navigation.RemovePage(page);
					i--;
				}

				for (int i = 0; i < shell.Navigation.ModalStack.Count; i++)
				{
					var page = shell.Navigation.ModalStack[i];

					if (page is null or TPage)
						continue;

					shell.Navigation.RemovePage(page);
					i--;
				}
			});
		}
	}
}