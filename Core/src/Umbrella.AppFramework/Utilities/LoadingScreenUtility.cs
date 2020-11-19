using System;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.AppFramework.Utilities.Abstractions;

namespace Umbrella.AppFramework.Utilities
{
	public class LoadingScreenUtility : ILoadingScreenUtility
	{
		private CancellationTokenSource? _cancellationTokenSource;
		private Action? _onShowAction;
		private Action? _onHideAction;

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