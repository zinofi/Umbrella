using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Umbrella.AppFramework.Security.Abstractions;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.AppFramework.Utilities.Constants;
using Umbrella.Utilities.Http;

namespace Umbrella.AppFramework.UI
{
	// Add documentation to make it clear why this implements INotifyPropertyChanged. Only Xamarin actually needs it although there's nothing to stop
	// Blazor users making use of it if so desired.
	public abstract class UmbrellaUIHandlerBase : INotifyPropertyChanged
	{
		protected ILogger Logger { get; }
		protected IDialogUtility DialogUtility { get; }
		protected IAppAuthHelper AuthHelper { get; }

		public UmbrellaUIHandlerBase(
			ILogger logger,
			IDialogUtility dialogUtility,
			IAppAuthHelper authHelper)
		{
			Logger = logger;
			DialogUtility = dialogUtility;
			AuthHelper = authHelper;
		}

		protected async Task<ClaimsPrincipal> GetClaimsPrincipalAsync() => await AuthHelper.GetCurrentClaimsPrincipalAsync();

		/// <summary>
		/// Shows a friendly error message for the specified <paramref name="problemDetails"/> using the <see cref="DialogUtility"/>.
		/// </summary>
		/// <param name="problemDetails">The problem details.</param>
		/// <param name="title">The title.</param>
		protected async Task ShowProblemDetailsErrorMessageAsync(HttpProblemDetails problemDetails, string title = "Error")
			=> await DialogUtility.ShowDangerMessageAsync(problemDetails?.Detail ?? DialogDefaults.UnknownErrorMessage, title);

		protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
		{
			if (EqualityComparer<T>.Default.Equals(backingStore, value))
				return false;

			backingStore = value;
			onChanged?.Invoke();
			OnPropertyChanged(propertyName);

			return true;
		}

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		#endregion
	}
}