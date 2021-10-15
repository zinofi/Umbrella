using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.UI;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.Utilities.Http.Exceptions;
using Umbrella.Utilities.WeakEventManager.Abstractions;
using Umbrella.Xamarin.Utilities.Abstractions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
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
		/// Gets the OpenUrlInternal command.
		/// </summary>
		public AsyncCommand<string?> OpenUrlInternalCommand { get; }

		/// <summary>
		/// Gets the OpenUrlExternal command.
		/// </summary>
		public AsyncCommand<string?> OpenUrlExternalCommand { get; }

		/// <summary>
		/// Gets the event manager.
		/// </summary>
		public IGlobalWeakEventManager EventManager { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewModelBase"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="authHelper">The authentication helper.</param>
		/// <param name="eventManager">The event manager.</param>
		/// <param name="dialogUtility">The dialog utility.</param>
		public ViewModelBase(
			ILogger logger,
			IDialogUtility dialogUtility,
			IAppAuthHelper authHelper,
			IGlobalWeakEventManager eventManager)
			: base(logger, dialogUtility, authHelper)
		{
			EventManager = eventManager;

			// TODO TEMP365/SPARK: Use the IUmbrellaCommandFactory here.
			// Add a new virtual property to specify network checks for reloading. Default to true.
			ReloadButtonCommand = new AsyncCommand(OnReloadButtonClicked, allowsMultipleExecutions: false);
			OpenUrlInternalCommand = new AsyncCommand<string?>(x => OpenUrlAsync(x, true), allowsMultipleExecutions: false);
			OpenUrlExternalCommand = new AsyncCommand<string?>(x => OpenUrlAsync(x, false), allowsMultipleExecutions: false);
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

		/// <summary>
		/// Opens the specified <paramref name="url"/>.
		/// </summary>
		/// <param name="url">The URL to open.</param>
		/// <param name="openInsideApp">
		/// If <see langword="true"/>, opens the specified <paramref name="url"/> inside the app; otherwise
		/// the <paramref name="url"/> will open in the default web browser.
		/// </param>
		/// <returns>An awaitable Task that completes when the operation completes.</returns>
		protected async Task OpenUrlAsync(string? url, bool openInsideApp)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(url))
					throw new Exception("The url is null or empty");

				await Browser.OpenAsync(url, openInsideApp ? BrowserLaunchMode.SystemPreferred : BrowserLaunchMode.External);
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { url, openInsideApp }, returnValue: true))
			{
				await DialogUtility.ShowDangerMessageAsync();
			}
		}

		/// <summary>
		/// Performs navigation from the current page to the specified path.
		/// </summary>
		/// <param name="path">The application path to navigate to.</param>
		/// <returns>An awaitable Task that completes when the operation completes.</returns>
		protected async Task NavigateToAppPathAsync(string path)
		{
			try
			{
				await Shell.Current.GoToAsync(path);
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { path }, returnValue: true))
			{
				await DialogUtility.ShowDangerMessageAsync();
			}
		}

		/// <summary>
		/// Registers an auto-unsubscribing event subscription with the specified <paramref name="eventName"/>.
		/// This replaces any existing subscriptions for the same named event. After the event is raised by a publisher,
		/// the subscription is automatically removed to prevent the <paramref name="eventHandler"/> being executed again.
		/// </summary>
		/// <param name="eventName">The name of the event to subscribe to.</param>
		/// <param name="eventHandler">The event handler.</param>
		/// <returns>A <see cref="Task"/> which completes after the subscription has been registered.</returns>
		protected async Task RegisterAutoUnsubscribingEventAsync(string eventName, Func<Task> eventHandler)
		{
			try
			{
				EventManager.RemoveAllEventHandlers(eventName);
				EventManager.AddEventHandler<Action>(async () =>
				{
					try
					{
						await eventHandler();
					}
					catch (UmbrellaHttpServiceConcurrencyException)
					{
						await DialogUtility.ShowDangerMessageAsync("The data on this page has changed since it was loaded. Please try again.");
					}
					catch (Exception exc) when (Logger.WriteError(exc))
					{
						await DialogUtility.ShowDangerMessageAsync();
					}
					finally
					{
						EventManager.RemoveAllEventHandlers(eventName);
					}
				},
				eventName);
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { eventName }))
			{
				EventManager.RemoveAllEventHandlers(eventName);
				await DialogUtility.ShowDangerMessageAsync();
			}
		}

		/// <summary>
		/// Registers an auto-unsubscribing event subscription with the specified <paramref name="eventName"/>.
		/// This replaces any existing subscriptions for the same named event. After the event is raised by a publisher,
		/// the subscription is automatically removed to prevent the <paramref name="eventHandler"/> being executed again.
		/// </summary>
		/// <typeparam name="TResult">The type of the result raised by a publisher and supplied as the argument to the <paramref name="eventHandler"/>.</typeparam>
		/// <param name="eventName">The name of the event to subscribe to.</param>
		/// <param name="eventHandler">The event handler.</param>
		/// <returns>A <see cref="Task"/> which completes after the subscription has been registered.</returns>
		protected async Task RegisterAutoUnsubscribingEventAsync<TResult>(string eventName, Func<TResult, Task> eventHandler)
		{
			try
			{
				EventManager.RemoveAllEventHandlers(eventName);
				EventManager.AddEventHandler<Func<TResult, Task>>(async result =>
				{
				   try
				   {
					   await eventHandler(result);
				   }
				   catch (UmbrellaHttpServiceConcurrencyException)
				   {
					   await DialogUtility.ShowDangerMessageAsync("The data on this page has changed since it was loaded. Please try again.");
				   }
				   catch (Exception exc) when (Logger.WriteError(exc))
				   {
					   await DialogUtility.ShowDangerMessageAsync();
				   }
				   finally
				   {
					   EventManager.RemoveAllEventHandlers(eventName);
				   }
				},
				eventName);
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { eventName }))
			{
				EventManager.RemoveAllEventHandlers(eventName);
				await DialogUtility.ShowDangerMessageAsync();
			}
		}
	}
}