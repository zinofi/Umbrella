using Umbrella.AppFramework.Utilities.Abstractions;

namespace Umbrella.AppFramework.Utilities;

/// <summary>
/// A utility used to track open dialogs.
/// </summary>
public class DialogTracker : IDialogTracker
{
	private readonly HashSet<int> _dialogList = new();

	/// <inheritdoc />
	public bool TrackOpen(int code) => _dialogList.Add(code);

	/// <inheritdoc />
	public void Close(int code) => _dialogList.Remove(code);

	/// <inheritdoc />
	public int GenerateCode(string message, string title, string? subTitle, params string[] buttonText)
	{
		int hashCode = -281079646;

		hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(message);
		hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(title);

		if (subTitle is not null)
			hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(subTitle);

		foreach (string text in buttonText)
		{
			hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(text);
		}

		return hashCode;
	}
}