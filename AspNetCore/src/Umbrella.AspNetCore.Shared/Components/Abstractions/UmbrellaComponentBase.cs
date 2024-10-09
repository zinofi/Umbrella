using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Umbrella.AspNetCore.Shared.Services.Abstractions;
using Umbrella.Utilities.Mapping.Abstractions;

namespace Umbrella.AspNetCore.Shared.Components.Abstractions;

/// <summary>
/// A base component to be used with Blazor components which contain commonly used functionality.
/// </summary>
/// <seealso cref="ComponentBase" />
/// <seealso cref="IAsyncDisposable"/>
[SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "False positive. The fields are disposed correctly.")]
public abstract class UmbrellaComponentBase : ComponentBase, IAsyncDisposable
{
	private CancellationTokenSource? _componentCancellationTokenSource;
	private CancellationTokenSource? _linkedCancellationTokenSource;
	private bool _disposedValue;

	[Inject]
	private ILoggerFactory LoggerFactory { get; set; } = null!;

	/// <summary>
	/// Gets the navigation manager.
	/// </summary>
	/// <remarks>
	/// Useful extension methods can be found inside <see cref="NavigationManagerExtensions"/>.
	/// </remarks>
	[Inject]
	protected NavigationManager Navigation { get; private set; } = null!;

	/// <summary>
	/// Gets the mapper.
	/// </summary>
	[Inject]
	protected IUmbrellaMapper Mapper { get; private set; } = null!;

	/// <summary>
	/// Gets the HTTP request aborted service.
	/// </summary>
	/// <remarks>
	/// When this component is running on server, this service will provide the <see cref="System.Threading.CancellationToken"/> for the current HTTP request.
	/// In Blazor WebAssembly, this service will provide a no-op implementation as there is no request to abort.
	/// </remarks>
	[Inject]
	protected IHttpRequestAbortedService HttpRequestAbortedService { get; private set; } = null!;

	/// <summary>
	/// Gets the logger.
	/// </summary>
	protected ILogger Logger { get; private set; } = null!;

	/// <summary>
	/// Gets the <see cref="ClaimsPrincipal"/> for the current user.
	/// </summary>
	protected static ClaimsPrincipal? User => ClaimsPrincipal.Current;

	/// <summary>
	/// Gets the cancellation token.
	/// </summary>
	protected CancellationToken CancellationToken
	{
		get
		{
			if (_linkedCancellationTokenSource is not null)
				return _linkedCancellationTokenSource.Token;

			_componentCancellationTokenSource = new CancellationTokenSource();
			_linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_componentCancellationTokenSource.Token, HttpRequestAbortedService.RequestAborted);

			return _linkedCancellationTokenSource.Token;
		}
	}

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		base.OnInitialized();

		Logger = LoggerFactory.CreateLogger(GetType());
	}

	/// <summary>
	/// Releases unmanaged and - optionally - managed resources.
	/// </summary>
	/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
	protected virtual async ValueTask DisposeAsync(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				if (_componentCancellationTokenSource is not null)
				{
					if (_componentCancellationTokenSource.IsCancellationRequested)
					{
#if NET8_0_OR_GREATER
						await _componentCancellationTokenSource.CancelAsync();
#else
						await Task.Yield();
						_componentCancellationTokenSource.Cancel();
#endif
					}

					_componentCancellationTokenSource.Dispose();
				}

				if (_linkedCancellationTokenSource is not null)
				{
					if (_linkedCancellationTokenSource.IsCancellationRequested)
					{
#if NET8_0_OR_GREATER
						await _linkedCancellationTokenSource.CancelAsync();
#else
						await Task.Yield();
						_linkedCancellationTokenSource.Cancel();
#endif
					}

					_linkedCancellationTokenSource.Dispose();
				}
			}

			_disposedValue = true;
		}
	}

	/// <inheritdoc/>
	public async ValueTask DisposeAsync()
	{
		await DisposeAsync(disposing: true);
		GC.SuppressFinalize(this);
	}
}