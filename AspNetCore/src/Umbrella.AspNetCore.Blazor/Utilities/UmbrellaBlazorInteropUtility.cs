using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Umbrella.AspNetCore.Blazor.Utilities.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Utilities;

/// <summary>
/// A utility containing core interop functionality between Blazor and JavaScript for features not yet supported
/// natively by Blazor.
/// </summary>
/// <seealso cref="IUmbrellaBlazorInteropUtility"/>
public class UmbrellaBlazorInteropUtility : IUmbrellaBlazorInteropUtility
{
	private readonly ILogger _logger;
	private readonly IJSRuntime _jsRuntime;
	private readonly DotNetObjectReference<UmbrellaBlazorInteropUtility> _interopReference;
	private readonly List<AwaitableBlazorEventHandler> _windowScrolledTopEventHandlerList = new();

	/// <inheritdoc />
	public event AwaitableBlazorEventHandler OnWindowScrolledTop
	{
		add
		{
			_windowScrolledTopEventHandlerList.Add(value);

			if (_windowScrolledTopEventHandlerList.Count is 1)
				_ = InitializeWindowScrolledTopAsync();
		}
		remove
		{
			_ = _windowScrolledTopEventHandlerList.Remove(value);

			if (_windowScrolledTopEventHandlerList.Count is 0)
				_ = DestroyWindowScrolledTopAsync();
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaBlazorInteropUtility"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	/// <param name="jsRuntime">The js runtime.</param>
	public UmbrellaBlazorInteropUtility(
		ILogger<UmbrellaBlazorInteropUtility> logger,
		IJSRuntime jsRuntime)
	{
		_logger = logger;
		_jsRuntime = jsRuntime;
		_interopReference = DotNetObjectReference.Create(this);
	}

	/// <inheritdoc />
	public async ValueTask SetPageTitleAsync(string pageTitle)
	{
		try
		{
			await _jsRuntime.InvokeVoidAsync("UmbrellaBlazorInterop.setPageTitle", pageTitle).ConfigureAwait(false);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { pageTitle }))
		{
			// Do nothing here
		}
	}

	/// <inheritdoc />
	public async ValueTask AnimateScrollToAsync(int scrollY, int offset = 0)
	{
		try
		{
			await _jsRuntime.InvokeVoidAsync("UmbrellaBlazorInterop.animateScrollToAsync", scrollY, offset).ConfigureAwait(false);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { scrollY }))
		{
			// Do nothing here
		}
	}

	/// <inheritdoc />
	public async ValueTask AnimateScrollToAsync(string elementSelector, int offset = 0)
	{
		try
		{
			await _jsRuntime.InvokeVoidAsync("UmbrellaBlazorInterop.animateScrollToAsync", elementSelector, offset).ConfigureAwait(false);
		}
		catch (Exception exc) when (_logger.WriteError(exc, new { elementSelector }))
		{
			// Do nothing here
		}
	}

	/// <inheritdoc />
	public async ValueTask AnimateScrollToBottomAsync()
	{
		try
		{
			await _jsRuntime.InvokeVoidAsync("UmbrellaBlazorInterop.animateScrollToBottomAsync").ConfigureAwait(false);
		}
		catch (Exception exc) when (_logger.WriteError(exc))
		{
			// Do nothing here
		}
	}

	/// <inheritdoc />
	[JSInvokable]
	public async ValueTask OnWindowScrolledTopAsync() => await Task.WhenAll(_windowScrolledTopEventHandlerList.Select(x => x.Invoke())).ConfigureAwait(false);

	private async Task InitializeWindowScrolledTopAsync() => await _jsRuntime.InvokeVoidAsync("UmbrellaBlazorInterop.initializeWindowScrolledTopAsync", _interopReference, 10).ConfigureAwait(false);

	private async Task DestroyWindowScrolledTopAsync() => await _jsRuntime.InvokeVoidAsync("UmbrellaBlazorInterop.destroyWindowScrolledTopAsync").ConfigureAwait(false);
}