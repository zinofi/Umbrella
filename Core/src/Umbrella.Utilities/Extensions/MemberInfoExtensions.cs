// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Humanizer;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Umbrella.Utilities.Extensions;

/// <summary>
/// Extension methods for the <see cref="MemberInfo"/> class.
/// </summary>
public static class MemberInfoExtensions
{
	/// <summary>
	/// Gets the display text for the given member by attempting to find a <see cref="DisplayAttribute"/> annotation.
	/// </summary>
	/// <param name="memberInfo">The member.</param>
	/// <returns>The text specified using the <see cref="DisplayAttribute"/> if it exists.</returns>
	public static string GetDisplayText(this MemberInfo memberInfo)
	{
		Guard.IsNotNull(memberInfo);

		return memberInfo.GetCustomAttribute<DisplayAttribute>()?.Name?.ToString() ?? memberInfo.Name.ToString().Humanize(LetterCasing.Title);
	}
}