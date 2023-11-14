using CommunityToolkit.Diagnostics;
using Umbrella.AppFramework.Services.Abstractions;

namespace Umbrella.AppFramework.Services;

/// <summary>
/// A utility used to track open dialogs.
/// </summary>
public class DialogTrackerService : IDialogTrackerService
{
	private readonly HashSet<int> _dialogList = [];

	/// <inheritdoc />
	public bool TrackOpen(int code) => _dialogList.Add(code);

	/// <inheritdoc />
	public void Close(int code) => _dialogList.Remove(code);

	/// <inheritdoc />
	public int GenerateCode(string message, string title, string? subTitle, params string[] buttonText)
	{
		Guard.IsNotNull(buttonText);

		int hashCode = -281079646;

		hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(message);
		hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(title);

		if (subTitle is not null)
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(subTitle);

		foreach (string text in buttonText)
		{
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(text);
		}

		return hashCode;
	}
}