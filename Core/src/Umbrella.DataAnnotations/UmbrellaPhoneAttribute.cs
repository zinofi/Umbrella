﻿using Umbrella.DataAnnotations.RegularExpressions;

namespace Umbrella.DataAnnotations;

/// <summary>
/// Specifies that a data field must match the regular expression for a UK Phone Number.
/// </summary>
/// <seealso cref="RegularExpressionAttribute" />
[AttributeUsage(AttributeTargets.Property)]
public sealed class UmbrellaPhoneAttribute : RegularExpressionAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaPhoneAttribute"/> class.
	/// </summary>
	public UmbrellaPhoneAttribute()
		: base(PhoneRegularExpressions.UKPhoneRegexString)
	{
	}
}