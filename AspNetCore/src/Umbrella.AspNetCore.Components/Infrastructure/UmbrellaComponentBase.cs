using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.Utilities.Constants;
using Umbrella.AspNetCore.Components.Components.Dialog.Abstractions;
using Umbrella.Utilities.Http;

namespace Umbrella.AspNetCore.Components.Infrastructure
{
	public abstract class UmbrellaComponentBase : ComponentBase
	{
		[Inject]
		private ILoggerFactory LoggerFactory { get; set; } = null!;

		[Inject]
		protected NavigationManager Navigation { get; private set; } = null!;

		[Inject]
		protected IUmbrellaDialogUtility DialogUtility { get; private set; } = null!;

		[Inject]
		protected IAppAuthHelper AuthHelper { get; private set; } = null!;

		protected ILogger Logger { get; private set; } = null!;

		/// <inheritdoc />
		protected override void OnInitialized()
		{
			base.OnInitialized();

			Logger = LoggerFactory.CreateLogger(GetType());
		}

		protected async Task<ClaimsPrincipal> GetClaimsPrincipalAsync() => await AuthHelper.GetCurrentClaimsPrincipalAsync();

		protected async Task ShowProblemDetailsErrorMessageAsync(HttpProblemDetails? problemDetails, string title = "Error")
			=> await DialogUtility.ShowDangerMessageAsync(problemDetails?.Detail ?? DialogDefaults.UnknownErrorMessage, title);
	}
}