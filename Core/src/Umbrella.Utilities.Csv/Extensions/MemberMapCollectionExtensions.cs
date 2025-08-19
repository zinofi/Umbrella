using CommunityToolkit.Diagnostics;
using CsvHelper.Configuration;
using Humanizer;

namespace Umbrella.Utilities.Csv.Extensions;

/// <summary>
/// Extensions for the <see cref="MemberMapCollection"/> class.
/// </summary>
public static class MemberMapCollectionExtensions
{
	/// <summary>
	/// Ensures that all member maps in the collection have user-friendly header names.
	/// </summary>
	/// <remarks>If a member map does not already have a name explicitly set, this method assigns a name based on
	/// the member's name, converted to a human-readable format with title casing.</remarks>
	/// <param name="memberMaps">The collection of member maps to process. Cannot be <see langword="null"/>.</param>
	public static void EnsureFriendlyHeaderNames(this MemberMapCollection memberMaps)
	{
		Guard.IsNotNull(memberMaps);

		foreach (var map in memberMaps)
		{
			if (map.Data.IsNameSet)
				continue;

			string? memberName = map.Data.Member?.Name;

			if (string.IsNullOrEmpty(memberName))
				continue;

			_ = map.Name(memberName.Humanize(LetterCasing.Title));
		}
	}
}