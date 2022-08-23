// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Exceptions;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.AppFramework.Utilities.Messages;

namespace Umbrella.AppFramework.Utilities;

/// <summary>
/// A utility used to show and hide an application loading screen.
/// </summary>
public class LoadingScreenUtility : ILoadingScreenUtility
{
	private readonly ILogger<LoadingScreenUtility> _logger;
	private CancellationTokenSource? _cancellationTokenSource;

	/// <inheritdoc />
	public event Action OnLoading
	{
		add => WeakReferenceMessenger.Default.Register<LoadingScreenStateChangedMessage>(value.Target, (_, _) => value.Invoke());
		remove => WeakReferenceMessenger.Default.Unregister<LoadingScreenStateChangedMessage>(value.Target);
	}

	/// <inheritdoc />
	public event Action OnShow
	{
		add => WeakReferenceMessenger.Default.Register<LoadingScreenStateChangedMessage>(value.Target, (_, _) => value.Invoke());
		remove => WeakReferenceMessenger.Default.Unregister<LoadingScreenStateChangedMessage>(value.Target);
	}

	/// <inheritdoc />
	public event Action OnHide
	{
		add => WeakReferenceMessenger.Default.Register<LoadingScreenStateChangedMessage>(value.Target, (_, _) => value.Invoke());
		remove => WeakReferenceMessenger.Default.Unregister<LoadingScreenStateChangedMessage>(value.Target);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="LoadingScreenUtility"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	public LoadingScreenUtility(ILogger<LoadingScreenUtility> logger)
	{
		_logger = logger;
	}

	/// <inheritdoc />
	public void Show(int delayMilliseconds = 500)
	{
		try
		{
			if (_cancellationTokenSource != null)
				return;

			_cancellationTokenSource = new CancellationTokenSource();

			_ = WeakReferenceMessenger.Default.Send(new LoadingScreenStateChangedMessage(Enumerations.LoadingScreenState.Requested));

			var token = _cancellationTokenSource.Token;

			_ = Task.Delay(delayMilliseconds, token).ContinueWith(x =>
			{
				if (!token.IsCancellationRequested)
					_ = WeakReferenceMessenger.Default.Send(new LoadingScreenStateChangedMessage(Enumerations.LoadingScreenState.Visible));
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
			_ = WeakReferenceMessenger.Default.Send(new LoadingScreenStateChangedMessage(Enumerations.LoadingScreenState.Hidden));
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