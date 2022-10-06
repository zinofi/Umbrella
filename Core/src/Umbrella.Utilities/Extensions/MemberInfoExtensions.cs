// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using CommunityToolkit.Diagnostics;
using Humanizer;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace Umbrella.Utilities.Extensions;

/// <summary>
/// Extension methods for the <see cref="MemberInfo"/> class.
/// </summary>
public static class MemberInfoExtensions
{
	/// <summary>
	/// Gets the display text for the given member by trying the following in order:
	/// <list type="bullet">
	/// <item>Use a <see cref="DisplayAttribute"/>.</item>
	/// <item>Use a <see cref="DisplayNameAttribute"/>.</item>
	/// <item>Use Humanizer to convert the member name to a friendly string using <see cref="LetterCasing.Title"/>.</item>
	/// </list>
	/// </summary>
	/// <param name="memberInfo">The member.</param>
	/// <returns>The display name according to the specified rules.</returns>
	public static string GetDisplayText(this MemberInfo memberInfo)
	{
		Guard.IsNotNull(memberInfo);

		return memberInfo.GetCustomAttribute<DisplayAttribute>()?.Name
			?? memberInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
			?? memberInfo.Name.Humanize(LetterCasing.Title);
	}
}