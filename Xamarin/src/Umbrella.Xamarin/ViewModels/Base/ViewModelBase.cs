using System;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.UI;
using Umbrella.AppFramework.Utilities.Abstractions;
using Xamarin.Forms;

namespace Spark.App.ViewModels.Base
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

		public bool IsRefreshing
		{
			get => _isRefreshing;
			set => SetProperty(ref _isRefreshing, value);
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
		}

		/// <summary>
		/// Sets the <see cref="IsBusy"/> and <see cref="IsRefreshing"/> flags to <see langword="false" />
		/// </summary>
		protected void ClearFlags() => IsBusy = IsRefreshing = false;
	}
}