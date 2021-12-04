// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;

namespace Umbrella.Utilities.Dating.Abstractions
{
	/// <summary>
	/// A provider used to get <see cref="DateTime"/> instances.
	/// </summary>
	/// <remarks>
	/// This can be used to avoid hard dependencies on the static method of the <see cref="DateTime"/>
	/// where the consuming type needs specific values for <see cref="DateTime.Now"/> and <see cref="DateTime.UtcNow"/>
	/// for unit testing purposes.
	/// </remarks>
	public interface IDateTimeProvider
	{
		/// <summary>
		/// Gets the current <see cref="DateTime"/> using the local timezone.
		/// </summary>
		DateTime Now { get; }

		/// <summary>
		/// Gets the current <see cref="DateTime"/> using the UTC timezone.
		/// </summary>
		DateTime UtcNow { get; }

		/// <summary>
		/// Gets the current date with the time component set to midnight. The <see cref="DateTime.Kind"/>
		/// property should return <see cref="DateTimeKind.Local"/>.
		/// </summary>
		DateTime Today { get; }
	}
}
