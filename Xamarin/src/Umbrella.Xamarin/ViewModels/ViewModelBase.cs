using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.UI;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.Xamarin.Utilities.Abstractions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Umbrella.Xamarin.ViewModels
{
	/// <summary>
	/// A base view model which all Xamarin view models should extend.
	/// </summary>
	/// <seealso cref="UmbrellaUIHandlerBase" />
	public abstract class ViewModelBase : UmbrellaUIHandlerBase
	{
		private bool _isBusy;
		private Page? _currentPage;
		private bool _isRefreshing;
		private LayoutState _currentState = LayoutState.Loading;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is refreshing.
		/// </summary>
		/// <remarks>
		/// This is usually bound to a control, e.g. a <see cref="RefreshView"/>, which means its value is set by the control.
		/// However, it needs to be manually set to <see langword="false"/> when the refresh operation has been completed
		/// in order for the UI to change state.
		/// </remarks>
		public bool IsRefreshing
		{
			get => _isRefreshing;
			set => SetProperty(ref _isRefreshing, value);
		}

		/// <summary>
		/// Gets or sets the current layout state. The initial state is <see cref="LayoutState.Loading"/>.
		/// </summary>
		public LayoutState CurrentState
		{
			get => _currentState;
			set => SetProperty(ref _currentState, value);
		}

		/// <summary>
		/// Gets or sets the current page.
		/// </summary>
		/// <remarks>This is required for ViewModels which need access to the current Xamarin page, e.g. it needs to be passed to the <see cref="IXamarinValidationUtility"/>.</remarks>
		/// <exception cref="Exception">The CurrentPage property has not been set.</exception>
		public Page? CurrentPage
		{
			get => _currentPage ?? throw new Exception("The CurrentPage property has not been set.");
			set => _currentPage = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is busy.
		/// </summary>
		/// <remarks>This is useful when implementing mutex code blocks, e.g. when loading items from a remote API and wanting to ensure duplicate requests don't take place.</remarks>
		public bool IsBusy
		{
			get => _isBusy;
			set => SetProperty(ref _isBusy, value);
		}

		/// <summary>
		/// Gets the reload button command.
		/// </summary>
		public AsyncCommand ReloadButtonCommand { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewModelBase"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="authHelper">The authentication helper.</param>
		/// <param name="dialogUtility">The dialog utility.</param>
		public ViewModelBase(
			ILogger logger,
			IDialogUtility dialogUtility,
			IAppAuthHelper authHelper)
			: base(logger, dialogUtility, authHelper)
		{
			// TODO TEMP365/SPARK: Use the IUmbrellaCommandFactory here.
			// Add a new virtual property to specify network checks for reloading. Default to true.
			ReloadButtonCommand = new AsyncCommand(OnReloadButtonClicked, allowsMultipleExecutions: false);
		}

		/// <summary>
		/// Sets the <see cref="IsBusy"/> and <see cref="IsRefreshing"/> flags to <see langword="false" />
		/// </summary>
		protected void ClearFlags() => IsBusy = IsRefreshing = false;

		/// <summary>
		/// Called when the <see cref="ReloadButtonCommand"/> is invoked. Unless overridden, this method
		/// just returns <see cref="Task.CompletedTask"/>.
		/// </summary>
		/// <returns>An awaitable Task that completes when the operation completes.</returns>
		protected virtual Task OnReloadButtonClicked() => Task.CompletedTask;
	}
}