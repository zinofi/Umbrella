using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Exceptions;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.Utilities.WeakEventManager.Abstractions;

namespace Umbrella.AppFramework.Utilities
{
	/// <summary>
	/// A utility used to show and hide an application loading screen.
	/// </summary>
	public class LoadingScreenUtility : ILoadingScreenUtility
	{
		private readonly ILogger<LoadingScreenUtility> _logger;
		private readonly IWeakEventManager _weakEventManager;
		private CancellationTokenSource? _cancellationTokenSource;

		/// <inheritdoc />
		public event Action OnLoading
		{
			add => _weakEventManager.AddEventHandler(value);
			remove => _weakEventManager.RemoveEventHandler(value);
		}

		/// <inheritdoc />
		public event Action OnShow
		{
			add => _weakEventManager.AddEventHandler(value);
			remove => _weakEventManager.RemoveEventHandler(value);
		}

		/// <inheritdoc />
		public event Action OnHide
		{
			add => _weakEventManager.AddEventHandler(value);
			remove => _weakEventManager.RemoveEventHandler(value);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LoadingScreenUtility"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="weakEventManagerFactory">The weak event manager factory.</param>
		public LoadingScreenUtility(
			ILogger<LoadingScreenUtility> logger,
			IWeakEventManagerFactory weakEventManagerFactory)
		{
			_logger = logger;
			_weakEventManager = weakEventManagerFactory.Create();
		}

		/// <inheritdoc />
		public void Show(int delayMilliseconds = 500)
		{
			try
			{
				if (_cancellationTokenSource != null)
					return;

				_cancellationTokenSource = new CancellationTokenSource();

				_weakEventManager.RaiseEvent(nameof(OnLoading));

				var token = _cancellationTokenSource.Token;

				Task.Delay(delayMilliseconds, token).ContinueWith(x =>
				{
					if (!token.IsCancellationRequested)
						_weakEventManager.RaiseEvent(nameof(OnShow));
				});
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { delayMilliseconds }, returnValue: true))
			{
				throw new UmbrellaAppFrameworkException("There has been a problem showing the loading screen.", exc);
			}
		}

		/// <inheritdoc />
		public void Hide()
		{
			try
			{
				_cancellationTokenSource?.Cancel();
				_weakEventManager.RaiseEvent(this, nameof(OnHide));
			}
			catch (Exception exc)
			{
				throw new UmbrellaAppFrameworkException("There has been a problem hiding the loading screen.", exc);
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