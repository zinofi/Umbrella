using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Umbrella.AspNetCore.Blazor.Utilities.Abstractions;

namespace Umbrella.AspNetCore.Blazor.Utilities
{
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
		private readonly List<AwaitableBlazorEventHandler> _windowScrolledTopEventHandlerList = new List<AwaitableBlazorEventHandler>();

		/// <inheritdoc />
		public event AwaitableBlazorEventHandler OnWindowScrolledTop
		{
			add
			{
				_windowScrolledTopEventHandlerList.Add(value);

				if (_windowScrolledTopEventHandlerList.Count is 1)
					InitializeWindowScrolledTop();
			}
			remove
			{
				_windowScrolledTopEventHandlerList.Remove(value);

				if (_windowScrolledTopEventHandlerList.Count is 0)
					DestroyWindowScrolledTop();
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
				await _jsRuntime.InvokeVoidAsync("UmbrellaBlazorInterop.setPageTitle", pageTitle);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { pageTitle }, returnValue: true))
			{
				// Do nothing here
			}
		}

		/// <inheritdoc />
		public async ValueTask AnimateScrollToAsync(int scrollY, int offset = 0)
		{
			try
			{
				await _jsRuntime.InvokeVoidAsync("UmbrellaBlazorInterop.animateScrollToAsync", scrollY, offset);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { scrollY }, returnValue: true))
			{
				// Do nothing here
			}
		}

		/// <inheritdoc />
		public async ValueTask AnimateScrollToAsync(string elementSelector, int offset = 0)
		{
			try
			{
				await _jsRuntime.InvokeVoidAsync("UmbrellaBlazorInterop.animateScrollToAsync", elementSelector, offset);
			}
			catch (Exception exc) when (_logger.WriteError(exc, new { elementSelector }, returnValue: true))
			{
				// Do nothing here
			}
		}

		/// <inheritdoc />
		public async ValueTask AnimateScrollToBottomAsync()
		{
			try
			{
				await _jsRuntime.InvokeVoidAsync("UmbrellaBlazorInterop.animateScrollToBottomAsync");
			}
			catch (Exception exc) when (_logger.WriteError(exc, returnValue: true))
			{
				// Do nothing here
			}
		}

		/// <inheritdoc />
		[JSInvokable]
		public async ValueTask OnWindowScrolledTopAsync() => await Task.WhenAll(_windowScrolledTopEventHandlerList.Select(x => x.Invoke()));

		private void InitializeWindowScrolledTop() => _jsRuntime.InvokeVoidAsync("UmbrellaBlazorInterop.initializeWindowScrolledTopAsync", _interopReference, 10);

		private void DestroyWindowScrolledTop() => _jsRuntime.InvokeVoidAsync("UmbrellaBlazorInterop.destroyWindowScrolledTopAsync");
	}
}