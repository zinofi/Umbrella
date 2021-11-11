using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.AppFramework.Utilities.Abstractions;
using Umbrella.Utilities.Networking.Abstractions;
using Umbrella.Xamarin.ObjectModel.Abstractions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Umbrella.Xamarin.ObjectModel
{
	public class UmbrellaCommandFactory : IUmbrellaCommandFactory
	{
		public UmbrellaCommandFactory(
			ILogger<UmbrellaCommandFactory> logger,
			IDialogUtility dialogUtility,
			INetworkConnectionStatusUtility networkConnectionStatusUtility)
		{
			Logger = logger;
			DialogUtility = dialogUtility;
			NetworkConnectionStatusUtility = networkConnectionStatusUtility;
		}

		protected ILogger<UmbrellaCommandFactory> Logger { get; }
		protected IDialogUtility DialogUtility { get; }
		protected INetworkConnectionStatusUtility NetworkConnectionStatusUtility { get; }

		/// <inheritdoc />
		public Command CreateCommand(Action execute, Func<bool>? canExecute = null) => new Command(execute, canExecute);

		/// <inheritdoc />
		public Command<T> CreateCommand<T>(Action<T> execute, Func<T, bool>? canExecute = null) => new Command<T>(execute, canExecute);

		/// <inheritdoc />
		public AsyncCommand CreateAsyncCommand(Func<Task> execute, bool checkNetworkConnection = false, Func<bool>? canExecute = null) => new AsyncCommand(execute!, () => CanExecute(canExecute, checkNetworkConnection), allowsMultipleExecutions: false);

		/// <inheritdoc />
		public AsyncCommand<T> CreateAsyncCommand<T>(Func<T, Task> execute, bool checkNetworkConnection = false, Func<bool>? canExecute = null) => new AsyncCommand<T>(execute!, () => CanExecute(canExecute, checkNetworkConnection), allowsMultipleExecutions: false);

		/// <inheritdoc />
		public AsyncValueCommand CreateAsyncValueCommand(Func<ValueTask> execute, bool checkNetworkConnection = false, Func<bool>? canExecute = null) => new AsyncValueCommand(execute!, () => CanExecute(canExecute, checkNetworkConnection), allowsMultipleExecutions: false);

		/// <inheritdoc />
		public AsyncValueCommand<T> CreateAsyncValueCommand<T>(Func<T, ValueTask> execute, bool checkNetworkConnection = false, Func<bool>? canExecute = null) => new AsyncValueCommand<T>(execute!, () => CanExecute(canExecute, checkNetworkConnection), allowsMultipleExecutions: false);

		private bool CanExecute(Func<bool>? innerCanExecute, bool checkNetworkConnection)
		{
			if (checkNetworkConnection && !NetworkConnectionStatusUtility.IsConnected)
			{
				// TODO: Create an options class to allow this to be configurable.
				_ = DialogUtility.ShowDangerMessageAsync("Your device is not connected to the internet. Please check your connection and try again.");

				return false;
			}

			return innerCanExecute == null || innerCanExecute();
		}
	}
}