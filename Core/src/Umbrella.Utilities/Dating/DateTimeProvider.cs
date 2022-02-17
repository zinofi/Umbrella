// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using Umbrella.Utilities.Dating.Abstractions;

namespace Umbrella.Utilities.Dating
{
	/// <summary>
	/// A provider used to get <see cref="DateTime"/> instances.
	/// </summary>
	/// <remarks>
	/// This can be used to avoid hard dependencies on the static method of the <see cref="DateTime"/>
	/// where the consuming type needs specific values for <see cref="DateTime.Now"/> and <see cref="DateTime.UtcNow"/>
	/// for unit testing purposes.
	/// </remarks>
	public class DateTimeProvider : IDateTimeProvider
	{
		/// <inheritdoc />
		public DateTime Now => DateTime.Now;

		/// <inheritdoc />
		public DateTime UtcNow => DateTime.UtcNow;

		/// <inheritdoc />
		public DateTime Today => DateTime.Today;
	}
}