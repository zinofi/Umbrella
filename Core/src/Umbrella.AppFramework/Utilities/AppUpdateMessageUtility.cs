using System;
using System.Threading.Tasks;
using Umbrella.AppFramework.Utilities.Abstractions;

namespace Umbrella.AppFramework.Utilities
{
	public class AppUpdateMessageUtility : IAppUpdateMessageUtility
	{
		private readonly IDialogUtility _dialogUtility;

		/// <inheritdoc />
		public event Func<bool, string, Task> OnShowAsync;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppUpdateMessageUtility"/> class.
		/// </summary>
		/// <param name="dialogUtility">The dialog utility.</param>
		public AppUpdateMessageUtility(IDialogUtility dialogUtility)
		{
			_dialogUtility = dialogUtility;
		}

		/// <inheritdoc />
		public async Task ShowAsync(bool updateRequired, string message)
		{
			var task = OnShowAsync?.Invoke(updateRequired, message);

			if (task != null)
				await task;

			await _dialogUtility.ShowMessageAsync(message, "Update Required");
		}
	}
}