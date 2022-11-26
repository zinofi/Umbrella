// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.Shared.Constants;
using Umbrella.AppFramework.Shared.Security;
using Umbrella.AppFramework.Shared.Security.Extensions;
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
	/// Gets the value as a string suitable for use as a value with <![CDATA[<option>]]> elements of a <see cref="InputSelect{TValue}"/> component.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <param name="value">The value.</param>
	/// <returns>The value converted to a string if not null; otherwise "-1"</returns>
	protected static string GetValueAsInputSelectString<TValue>(TValue? value) where TValue : struct => value?.ToString() ?? "-1";

	/// <summary>
	/// Sets a property value on the specified model using an income string value used with an <![CDATA[<option>]]> element of a <see cref="InputSelect{TValue}"/> component
	/// using the specified <paramref name="assignor"/> delegate.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <param name="model">The model.</param>
	/// <param name="value">The value.</param>
	/// <param name="assignor">The assignor.</param>
	/// <remarks>The string <paramref name="value"/> is converted to <typeparamref name="TValue"/> before assignment to the property on the specified <paramref name="model"/>.</remarks>
	protected static void SetValueFromInputSelectString<TValue>(object? model, string? value, Action<TValue?> assignor)
		where TValue : struct
	{
		if (model is null)
			return;

		object? convertedValue = null;

		if (typeof(TValue).IsEnum)
		{
			if (Enum.TryParse<TValue>(value, out var enumValue) && Enum.IsDefined(typeof(TValue), enumValue))
			{
				convertedValue = enumValue;
			}
		}
		else
		{
			convertedValue = value == "-1" ? null : Convert.ChangeType(value, typeof(TValue));
		}

		if (convertedValue is null or default(object?))
		{
			assignor(null);
		}
		else
		{
			assignor((TValue)convertedValue);
		}
	}

	/// <summary>
	/// Appends the a file access token as a query string parameter with the key <see cref="AppQueryStringKeys.FileAccessToken" /> using the value stored
	/// using the <see cref="UmbrellaAppClaimType.FileAccessToken "/> claim, if it exists on the current <see cref="ClaimsPrincipal"/>.
	/// </summary>
	/// <param name="uri">The URI to append the token to.</param>
	/// <returns>The URI with the appended token if the <see cref="UmbrellaAppClaimType.FileAccessToken" /> claim exists; otherwise the value of the <paramref name="uri"/> parameter.</returns>
	public async Task<string> AppendFileAccessTokenAsync(string uri)
	{
		var claimsPrincipal = await GetClaimsPrincipalAsync();
		string? accessToken = claimsPrincipal.GetFileAccessToken();

		return string.IsNullOrWhiteSpace(accessToken) ? uri : QueryHelpers.AddQueryString(uri, AppQueryStringKeys.FileAccessToken, accessToken);
	}

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