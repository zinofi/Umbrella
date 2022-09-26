// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.Utilities.Constants;
using Umbrella.AspNetCore.Blazor.Components.Dialog.Abstractions;
using Umbrella.AspNetCore.Blazor.Enumerations;
using Umbrella.AspNetCore.Blazor.Extensions;
using Umbrella.Utilities.Http;

namespace Umbrella.AspNetCore.Blazor.Infrastructure;

/// <summary>
/// A base component to be used with Blazor components which contain commonly used functionality.
/// </summary>
/// <seealso cref="ComponentBase" />
public abstract class UmbrellaComponentBase : ComponentBase, IDisposable
{
	private bool _disposedValue;
	private readonly Lazy<CancellationTokenSource> _cancellationTokenSource = new(() => new());

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
	/// Gets the dialog utility.
	/// </summary>
	[Inject]
	protected IUmbrellaDialogUtility DialogUtility { get; private set; } = null!;

	/// <summary>
	/// Gets the authentication helper.
	/// </summary>
	[Inject]
	protected IAppAuthHelper AuthHelper { get; private set; } = null!;

	/// <summary>
	/// Gets the logger.
	/// </summary>
	protected ILogger Logger { get; private set; } = null!;

	/// <summary>
	/// Gets or sets the current layout state. The initial state is <see cref="LayoutState.Loading"/>.
	/// </summary>
	protected LayoutState CurrentState { get; set; } = LayoutState.Loading;

	/// <summary>
	/// Gets the cancellation token.
	/// </summary>
	protected CancellationToken CancellationToken => _cancellationTokenSource.Value.Token;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		base.OnInitialized();

		Logger = LoggerFactory.CreateLogger(GetType());
	}

	/// <summary>
	/// Gets the claims principal for the current user.
	/// </summary>
	/// <returns>The claims principal.</returns>
	protected ValueTask<ClaimsPrincipal> GetClaimsPrincipalAsync() => AuthHelper.GetCurrentClaimsPrincipalAsync();

	/// <summary>
	/// Shows the problem details error message. If this does not exist, the error message defaults to <see cref="DialogDefaults.UnknownErrorMessage"/>.
	/// </summary>
	/// <param name="problemDetails">The problem details.</param>
	/// <param name="title">The title.</param>
	protected ValueTask ShowProblemDetailsErrorMessageAsync(HttpProblemDetails? problemDetails, string title = "Error")
		=> DialogUtility.ShowDangerMessageAsync(problemDetails?.Detail ?? DialogDefaults.UnknownErrorMessage, title);

	/// <summary>
	/// Reloads the component primarily in response to an error during the initial loading phase.
	/// </summary>
	/// <remarks>Defaults to <see cref="ComponentBase.OnInitializedAsync"/></remarks>
	/// <returns>An awaitable Task that completed when this operation has completed.</returns>
	protected virtual Task ReloadAsync() => OnInitializedAsync();

	/// <summary>
	/// Releases unmanaged and - optionally - managed resources.
	/// </summary>
	/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				if (_cancellationTokenSource.IsValueCreated)
				{
					_cancellationTokenSource.Value.Cancel();
					_cancellationTokenSource.Value.Dispose();
				}
			}

			_disposedValue = true;
		}
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}