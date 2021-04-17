using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.Utilities.Constants;
using Umbrella.AspNetCore.Blazor.Components.Dialog.Abstractions;
using Umbrella.AspNetCore.Blazor.Enumerations;
using Umbrella.AspNetCore.Blazor.Extensions;
using Umbrella.Utilities.Http;

namespace Umbrella.AspNetCore.Blazor.Infrastructure
{
	/// <summary>
	/// A base component to be used with Blazor components which contain commonly used functionality.
	/// </summary>
	/// <seealso cref="ComponentBase" />
	public abstract class UmbrellaComponentBase : ComponentBase
	{
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
		/// <returns>An awaitable Task that completed when this operation has completed.</returns>
		protected virtual Task ReloadAsync() => throw new NotImplementedException("Reloading has not been implemented.");
	}
}