namespace Umbrella.AppFramework.Utilities.Abstractions
{
	/// <summary>
	/// A utility used to track open dialogs.
	/// </summary>
	public interface IDialogTracker
	{
		/// <summary>
		/// Marks the specified dialog as having been closed.
		/// </summary>
		/// <param name="code">The code.</param>
		void Close(int code);

		/// <summary>
		/// Generates a code that can identify a dialog.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="title">The title.</param>
		/// <param name="subTitle">The sub title.</param>
		/// <param name="buttonText">The button text.</param>
		/// <returns>The code.</returns>
		int GenerateCode(string message, string title, string? subTitle, params string[] buttonText);

		/// <summary>
		/// Tracks the opening of a dialog.
		/// </summary>
		/// <param name="code">The code.</param>
		/// <returns><see langword="true"/> if the dialog being opened was not already open; otherwise <see langword="false"/>.</returns>
		bool TrackOpen(int code);
	}
}