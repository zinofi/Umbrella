﻿using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Umbrella.AspNetCore.Blazor.Services.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Services;

/// <summary>
/// An event aggregator used to subscribe to native browser events not supported by Blazor.
/// </summary>
/// <remarks>
/// This type is registered as a transient service and when disposed it will remove all subscriptions.
/// </remarks>
/// <seealso cref="IAsyncDisposable" />
/// <seealso cref="IBrowserEventAggregator" />
internal sealed class BrowserEventAggregator : IAsyncDisposable, IBrowserEventAggregator
{
	private readonly ILogger _logger;
	private readonly IJSRuntime _jsruntime;
	private readonly Dictionary<string, Func<ValueTask>> _callbackDictionary = new();
	private readonly DotNetObjectReference<BrowserEventAggregator> _dotNetObjectReference;
	private readonly string _id = Guid.NewGuid().ToString();

	/// <summary>
	/// Initializes a new instance of the <see cref="BrowserEventAggregator"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="jsruntime">The jsruntime.</param>
	public BrowserEventAggregator(
		ILogger<BrowserEventAggregator> logger,
		IJSRuntime jsruntime)
	{
		_logger = logger;
		_jsruntime = jsruntime;
		_dotNetObjectReference = DotNetObjectReference.Create(this);
	}

	/// <inheritdoc/>
	public async ValueTask SubscribeAsync(string eventName, Func<ValueTask> callback, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();

		try
		{
			if (_logger.IsEnabled(LogLevel.Debug))
				_logger.LogDebug("Subscribing event \"{EventName}\" with id: {Id}", eventName, _id);

			if (_callbackDictionary.TryGetValue(eventName, out Func<ValueTask>? result))
				throw new InvalidOperationException("An event subscription with the same name has already been added.");

			_callbackDictionary.Add(eventName, callback);

			await _jsruntime.InvokeVoidAsync("UmbrellaBlazorInterop.browserEventAggregator.addEventListener", cancellationToken, _id, eventName, _dotNetObjectReference);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { eventName }))
		{
			throw new UmbrellaBlazorException("There has been a problem subscribing to the event.", exc);
		}
	}

	/// <inheritdoc/>
	[JSInvokable]
	public async ValueTask PublishAsync(string eventName)
	{
		try
		{
			if (_logger.IsEnabled(LogLevel.Debug))
				_logger.LogDebug("Publishing event \"{EventName}\" with id: {Id}", eventName, _id);

			if (_callbackDictionary.TryGetValue(eventName, out var item))
				await item();
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { eventName }))
		{
			throw new UmbrellaBlazorException("There has been a problem publishing the event.", exc);
		}
	}

	/// <inheritdoc />
	public async ValueTask DisposeAsync()
	{
		try
		{
			foreach (string eventName in _callbackDictionary.Keys)
			{
				if (_logger.IsEnabled(LogLevel.Debug))
					_logger.LogDebug("Unsubscribing event \"{EventName}\" with id: {Id}", eventName, _id);

				await _jsruntime.InvokeVoidAsync("UmbrellaBlazorInterop.browserEventAggregator.removeEventListener", _id, eventName, _dotNetObjectReference);
			}
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			throw new UmbrellaBlazorException("There has been a problem disposing of this instance.", exc);
		}
	}
}