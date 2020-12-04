using System;

namespace Umbrella.AppFramework.Utilities.Abstractions
{
	/// <summary>
	/// A utility used to show and hide an application loading screen.
	/// </summary>
	public interface ILoadingScreenUtility
	{
		/// <summary>
		/// Occurs when the <see cref="Show(int)"/> method is called but before it is displayed because of any specified delay.
		/// </summary>
		event Action OnLoading;

		/// <summary>
		/// Occurs when the loading screen is shown.
		/// </summary>
		event Action OnShow;

		/// <summary>
		/// Occurs when the loading screen is hidden.
		/// </summary>
		event Action OnHide;

		/// <summary>
		/// Shows the loading screen after a delay of <paramref name="delayMilliseconds"/>. If the <see cref="Hide"/> method is
		/// called within the specified delay, the loading screen will not be displayed.
		/// </summary>
		/// <param name="delayMilliseconds">The delay in milliseconds.</param>
		void Show(int delayMilliseconds = 500);

		/// <summary>
		/// Hides the loading screen.
		/// </summary>
		void Hide();
	}
}