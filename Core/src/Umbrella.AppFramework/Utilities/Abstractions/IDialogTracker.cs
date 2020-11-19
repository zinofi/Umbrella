namespace Umbrella.AppFramework.Utilities.Abstractions
{
	public interface IDialogTracker
	{
		void Close(int code);
		int GenerateCode(string message, string title, string? subTitle, params string[] buttonText);
		bool TrackOpen(int code);
	}
}