using System;

namespace Umbrella.AppFramework.Utilities.Abstractions
{
	public interface ILoadingScreenUtility
	{
		event Action OnShow;
		event Action OnHide;
		void Show(int delayMilliseconds = 250);
		void Hide();
	}
}