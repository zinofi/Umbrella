using System;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.AppFramework.Utilities.Abstractions;

namespace Umbrella.AppFramework.Utilities
{
	// TODO V4: The events in here only allow one to be attached at a time. This is because we need to find a way to elegantly
	// detach event handlers in Xamarin. IDisposable on the UmbrellaActivityIndicatorView content view doesn't work.
	// Need to invent a mechanism to allow init and destroy like functionality with auto-discovery. This will do for
	// now though. Only really a problem when we hit a use case when 2 event handlers need to be attached at the same time.

	/// <summary>
	/// A utility used to show and hide an application loading screen.
	/// </summary>
	public class LoadingScreenUtility : ILoadingScreenUtility
	{
		private CancellationTokenSource? _cancellationTokenSource;
		private Action? _onLoadingAction;
		private Action? _onShowAction;
		private Action? _onHideAction;

		/// <inheritdoc />
		public event Action? OnLoading
		{
			add => _onLoadingAction = value;
			remove => _onLoadingAction = null;
		}

		/// <inheritdoc />
		public event Action OnShow
		{
			add => _onShowAction = value;
			remove => _onShowAction = null;
		}

		/// <inheritdoc />
		public event Action OnHide
		{
			add => _onHideAction = value;
			remove => _onHideAction = null;
		}

		/// <inheritdoc />
		public void Show(int delayMilliseconds = 250)
		{
			if (_cancellationTokenSource != null)
				return;

			_cancellationTokenSource = new CancellationTokenSource();

			_onLoadingAction?.Invoke();

			if (_onShowAction != null)
			{
				var token = _cancellationTokenSource.Token;

				Task.Delay(delayMilliseconds, token).ContinueWith(x =>
				{
					if (!token.IsCancellationRequested)
						_onShowAction();
				});
			}
		}

		/// <inheritdoc />
		public void Hide()
		{
			try
			{
				if (_onHideAction != null)
				{
					_cancellationTokenSource?.Cancel();
					_onHideAction();
				}
			}
			finally
			{
				if (_cancellationTokenSource != null)
				{
					_cancellationTokenSource.Dispose();
					_cancellationTokenSource = null;
				}
			}
		}
	}
}